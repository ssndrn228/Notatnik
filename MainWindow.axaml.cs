using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace notatnik;

public partial class MainWindow : Window
{
    private static readonly string NotesFilePath = Path.Combine(FindProjectDirectory(), "notatki.md");
    private static readonly Regex NoteHeading = new(
        @"^## (?<date>\d{4}-\d{2}-\d{2} \d{2}:\d{2}) — (?<title>.*)$",
        RegexOptions.Compiled);

    private readonly ObservableCollection<Note> _notes = [];
    private Note? _selectedNote;
    private bool _updatingSelection;

    public MainWindow()
    {
        InitializeComponent();
        NotesList.ItemsSource = _notes;
        LoadNotes();
        UpdateNoteCount();
    }

    private static string FindProjectDirectory()
    {
        foreach (var startPath in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(startPath);
            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "notatnik.csproj")))
                    return directory.FullName;

                directory = directory.Parent;
            }
        }

        return Environment.CurrentDirectory;
    }

    private void LoadNotes()
    {
        if (!File.Exists(NotesFilePath))
            return;

        try
        {
            var fileContents = File.ReadAllText(NotesFilePath, Encoding.UTF8);
            foreach (var note in ParseNotes(fileContents))
                _notes.Add(note);
        }
        catch (IOException)
        {
            ShowStatus("Nie udało się odczytać pliku notatki.md.", isError: true);
        }
        catch (UnauthorizedAccessException)
        {
            ShowStatus("Brak uprawnień do odczytu pliku notatki.md.", isError: true);
        }
    }

    private static IEnumerable<Note> ParseNotes(string fileContents)
    {
        using var reader = new StringReader(fileContents);
        var notes = new List<Note>();
        Note? currentNote = null;
        var contentLines = new List<string>();
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            var match = NoteHeading.Match(line);
            if (match.Success &&
                DateTime.TryParseExact(
                    match.Groups["date"].Value,
                    "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out var savedAt))
            {
                if (currentNote is not null)
                {
                    if (contentLines.Count > 0 && contentLines[^1].Length == 0)
                        contentLines.RemoveAt(contentLines.Count - 1);
                    currentNote.Content = string.Join(Environment.NewLine, contentLines);
                    notes.Add(currentNote);
                }

                currentNote = new Note
                {
                    Title = match.Groups["title"].Value,
                    SavedAt = new DateTimeOffset(savedAt)
                };
                contentLines.Clear();
            }
            else if (currentNote is not null)
            {
                contentLines.Add(line);
            }
        }

        if (currentNote is not null)
        {
            currentNote.Content = string.Join(Environment.NewLine, contentLines);
            notes.Add(currentNote);
        }

        return notes;
    }

    private void SaveNote_Click(object? sender, RoutedEventArgs e)
    {
        var title = TitleInput.Text?.Trim();
        var content = ContentInput.Text;
        if (string.IsNullOrWhiteSpace(title))
        {
            ShowStatus("Wpisz tytuł notatki.", isError: true);
            TitleInput.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            ShowStatus("Wpisz treść notatki.", isError: true);
            ContentInput.Focus();
            return;
        }

        title = title.Replace("\r", " ").Replace("\n", " ");
        var note = new Note
        {
            Title = title,
            Content = content.TrimEnd('\r', '\n'),
            SavedAt = DateTimeOffset.Now
        };
        var updatedNotes = _notes
            .Where(existing => !ReferenceEquals(existing, _selectedNote))
            .Prepend(note)
            .ToList();

        try
        {
            var temporaryPath = NotesFilePath + ".tmp";
            File.WriteAllText(temporaryPath, FormatNotes(updatedNotes), new UTF8Encoding(false));
            File.Move(temporaryPath, NotesFilePath, overwrite: true);

            _notes.Clear();
            foreach (var savedNote in updatedNotes)
                _notes.Add(savedNote);
            _selectedNote = note;
            _updatingSelection = true;
            NotesList.SelectedItem = note;
            _updatingSelection = false;
            UpdateNoteCount();
            ShowStatus("Notatka została zapisana.");
        }
        catch (IOException)
        {
            ShowStatus("Nie udało się zapisać notatki.md. Sprawdź dostęp do pliku.", isError: true);
        }
        catch (UnauthorizedAccessException)
        {
            ShowStatus("Brak uprawnień do zapisu pliku notatki.md.", isError: true);
        }
    }

    private static string FormatNotes(IEnumerable<Note> notes)
    {
        var entries = notes.Select(note =>
        {
            var heading = $"## {note.SavedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)} — {note.Title}";
            return $"{heading}{Environment.NewLine}{note.Content}";
        });
        var contents = string.Join($"{Environment.NewLine}{Environment.NewLine}", entries);
        return contents.Length == 0 ? string.Empty : contents + Environment.NewLine;
    }

    private void NewNote_Click(object? sender, RoutedEventArgs e)
    {
        _updatingSelection = true;
        NotesList.SelectedItem = null;
        _updatingSelection = false;
        _selectedNote = null;
        TitleInput.Clear();
        ContentInput.Clear();
        ShowStatus("Utwórz nową notatkę.");
        TitleInput.Focus();
    }

    private void NotesList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_updatingSelection || NotesList.SelectedItem is not Note note)
            return;

        _selectedNote = note;
        TitleInput.Text = note.Title;
        ContentInput.Text = note.Content;
        ShowStatus($"Otwarto notatkę „{note.Title}”.");
    }

    private void UpdateNoteCount()
    {
        NoteCountText.Text = _notes.Count.ToString(CultureInfo.InvariantCulture);
    }

    private void ShowStatus(string message, bool isError = false)
    {
        StatusText.Text = message;
        StatusText.Foreground = isError
            ? Avalonia.Media.Brushes.Firebrick
            : Avalonia.Media.Brushes.DarkSlateGray;
    }
}

public sealed class Note
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset SavedAt { get; set; } = DateTimeOffset.Now;

    public string Preview => Content.Length > 90 ? Content[..90] + "…" : Content;
    public string SavedAtLabel => $"Zapisano: {SavedAt.ToLocalTime():dd.MM.yyyy, HH:mm}";
}
