# Runda 5

## Analiza promptu

Agent wskazał, że pierwotna prośba nie określała:
- lokalizacji i formatu pliku;
- sposobu wczytywania notatek po ponownym uruchomieniu;
- zasad dodawania nowych notatek;
- walidacji pustych pól;
- zachowania listy notatek;
- obsługi błędów odczytu i zapisu;
- zachowania istniejącego projektu.

## Zmiany zaproponowane przez agenta

Agent zmienił:
- MainWindow.axaml — etykietę przycisku na „Zapisz”;
- MainWindow.axaml.cs — dodawanie nowych notatek na początku listy, czyszczenie pól po zapisie i odznaczanie wybranej notatki.

Kompilacja: 0 błędów, 0 ostrzeżeń.

## Test aplikacji

Sprawdzono:
- wczytywanie wcześniejszych notatek;
- wyświetlanie treści wybranej notatki;
- dodawanie nowej notatki;
- zachowanie wcześniejszych notatek;
- kolejność od najnowszej;
- zachowanie polskich znaków.

Wszystkie testy zakończyły się powodzeniem.

## Rzeczywista zawartość pliku notatki.md

## 2026-10-31 20:06 — Test rundy 5
To jest nowa notatka. ąęłżźóńćś

## 2026-10-31 19:55 — Plan dnia
nauka
zakupy
odpoczynek

## 2026-10-31 19:40 — Pomysł na projekt
minutnik z historią sesji

## 2026-10-31 19:40 — Zakupy
mleko
chleb
