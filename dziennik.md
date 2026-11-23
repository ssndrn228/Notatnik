# Dziennik

## Wpis 1. Refleksja po Budowie 2

W poprzednim zadaniu zaakceptowałam kod bez dokładnego sprawdzenia, czy wszystko działa tak, jak powinno. Nie sprawdziłam dokładnie każdej instrukcji i założyłam, że skoro kod wygląda poprawnie, to musi działać. Teraz wiem, że trzeba sprawdzać kod, uruchamiać program i testować zapis danych.

## Wpis 2. Sprawdzenie obecnego projektu

- **Budowanie:** `dotnet build` zakończyło się poprawnie.
- **Uruchamianie:** `dotnet run` uruchomiło aplikację i wyświetliło zapisane notatki.
- **Zapisywanie:** dodałam tekst do notatki, zapisałam go i sprawdziłam plik `notatki.md`. Nowa treść oraz wcześniejsze notatki pozostały w pliku. Po ponownym uruchomieniu aplikacji zapis nadal był widoczny.

## Sprawdzenie dokumentacji

Sprawdziłam klasę `ObservableCollection<T>` w dokumentacji Microsoftu.

- **Data sprawdzenia:** 23.11.2026
- **Link:** https://learn.microsoft.com/pl-pl/dotnet/api/system.collections.objectmodel.observablecollection-1

Klasa ta przechowuje kolekcję elementów i informuje interfejs użytkownika o dodaniu lub usunięciu elementów. Dzięki temu lista notatek może automatycznie się aktualizować.