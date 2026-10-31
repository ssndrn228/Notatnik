using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace notatnik;

public partial class MainWindow : Window
{
    private static readonly string NotesDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Notatnik");
    private static readonly string NotesFilePath = Path.Combine(NotesDirectory, "notatki.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

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

    private void LoadNotes()
    {
        if (!File.Exists(NotesFilePath))
            return;

        try
        {
            var notes = JsonSerializer.Deserialize<List<Note>>(File.ReadAllText(NotesFilePath));
            if (notes is not null)
            {
                foreach (var note in notes.OrderByDescending(note => note.SavedAt))
                    _notes.Add(note);
            }
        }
        catch (JsonException)
        {
            StatusText.Text = "Plik z notatkami ma nieprawidłowy format. Zapisz notatkę, aby go zastąpić.";
            StatusText.Foreground = Avalonia.Media.Brushes.DarkOrange;
        }
        catch (IOException)
        {
            StatusText.Text = "Nie udało się odczytać pliku z notatkami.";
            StatusText.Foreground = Avalonia.Media.Brushes.DarkOrange;
        }
        catch (UnauthorizedAccessException)
        {
            StatusText.Text = "Brak uprawnień do odczytu pliku z notatkami.";
            StatusText.Foreground = Avalonia.Media.Brushes.DarkOrange;
        }
    }

    private void SaveNote_Click(object? sender, RoutedEventArgs e)
    {
        var title = TitleInput.Text?.Trim();
        var content = ContentInput.Text?.Trim();
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

        var note = new Note
        {
            Title = title,
            Content = content,
            SavedAt = DateTimeOffset.Now
        };
        var updatedNotes = _notes
            .Where(existing => !ReferenceEquals(existing, _selectedNote))
            .Prepend(note)
            .ToList();

        try
        {
            Directory.CreateDirectory(NotesDirectory);
            var temporaryPath = NotesFilePath + ".tmp";
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(updatedNotes, JsonOptions));
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
            ShowStatus("Nie udało się zapisać notatki. Sprawdź dostęp do dysku.", isError: true);
        }
        catch (UnauthorizedAccessException)
        {
            ShowStatus("Brak uprawnień do zapisu pliku z notatkami.", isError: true);
        }
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
        NoteCountText.Text = _notes.Count.ToString();
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
