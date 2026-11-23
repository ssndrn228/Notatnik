# Rozbierz to

## 1. Timer zmienia element interfejsu z niewłaściwego wątku

W kodzie timer `System.Timers.Timer` wykonuje zdarzenie `Elapsed` w osobnym wątku. W tym zdarzeniu kod bezpośrednio zmienia tekst `StatusText.Text`.

- **Rodzaj halucynacji:** coś, co istnieje, ale jest użyte nieprawidłowo.
- **Kiedy występuje:** podczas działania programu.
- **Jak sprawdzić:** uruchomić program i poczekać na wykonanie timera. Można wtedy otrzymać błąd związany ze zmianą interfejsu z niewłaściwego wątku.
- **Czy występuje w moim Notatniku:** nie. Mój program nie używa `System.Timers.Timer` ani zdarzenia `Elapsed`.
- **Jak sprawdziłam:** przejrzałam plik `MainWindow.axaml.cs` i uruchomiłam aplikację.

## 2. Zapisywanie usuwa poprzednią zawartość pliku

Instrukcja `File.WriteAllText(path, NoteBox.Text)` zastępuje całą zawartość pliku nowym tekstem. Przez to poprzednia notatka może zostać utracona.

- **Rodzaj halucynacji:** coś, co istnieje, ale jest użyte niezgodnie z założeniami programu.
- **Kiedy występuje:** po kilku zapisach.
- **Jak sprawdzić:** zapisać jedną notatkę, potem drugą i sprawdzić, czy pierwsza nadal znajduje się w pliku.
- **Czy występuje w moim Notatniku:** nie.
- **Jak sprawdziłam:** zapisałam nowy tekst, a następnie sprawdziłam plik `notatki.md`. Nowa treść i wszystkie wcześniejsze notatki nadal były zapisane.

## 3. Komentarz mówi o obsłudze błędów, której nie ma

Komentarz w kodzie informuje, że obsługa błędów zapisu znajduje się w środku metody. Jednak w kodzie nie ma `try/catch`, który obsługiwałby takie błędy.

- **Rodzaj halucynacji:** fałszywa informacja w komentarzu.
- **Kiedy występuje:** podczas czytania kodu. Błąd może ujawnić się podczas zapisu, jeśli wystąpi problem.
- **Jak sprawdzić:** przeczytać metodę `SaveButton_Click` i sprawdzić, czy znajduje się w niej `try/catch`.
- **Czy występuje w moim Notatniku:** nie.
- **Jak sprawdziłam:** sprawdziłam metodę `SaveNote_Click` w pliku `MainWindow.axaml.cs`. Mój program posiada blok `try/catch`, który obsługuje błędy zapisu.