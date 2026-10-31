\# Notatnik



Projekt przedstawia aplikację Notatnik napisaną w C# z wykorzystaniem Avalonia UI.



\## Przebieg eksperymentu



W rundzie 1 użyto krótkiego prompta bez dodatkowych wymagań. Aplikacja zapisywała notatki w formacie JSON.



W rundzie 2 dokładnie określono format i miejsce zapisu. Notatki zaczęły być zapisywane w pliku `notatki.md`, w formacie Markdown, z najnowszą notatką na górze.



W rundzie 3 dodano jeden przykład oczekiwanego pliku. Agent nie zmienił kodu, ponieważ aplikacja spełniała już wymagania.



W rundzie 4 dodano trzy różne przykłady notatek. Agent wprowadził niewielką zmianę w wyszukiwaniu folderu projektu.



W rundzie 5 poproszono AI o ulepszenie pierwotnego prompta. Po zastosowaniu poprawionej wersji agent zmienił między innymi sposób dodawania notatek i czyszczenia pól po zapisie.



\## Powtórzenie



Powtórzono prompt z rundy 1 w nowej rozmowie. Agent nie wprowadził zmian, ponieważ aplikacja posiadała już wymagane funkcje. Kompilacja zakończyła się powodzeniem.



Wynik powtórzenia różnił się od pierwszej rundy, ponieważ agent pracował na projekcie zmienionym w poprzednich rundach. Różnice między rundami 1–4 mogły wynikać również z niedeterministycznego działania modeli AI, a nie tylko ze zmian promptów.



\## Własna obsługa błędów



Samodzielnie dodano blok `try-catch` obsługujący błędy zapisu. Przetestowano działanie aplikacji przy pliku ustawionym jako tylko do odczytu. Aplikacja wyświetliła komunikat o błędzie i pozostała otwarta. Po przywróceniu uprawnień zapis działał poprawnie.



\## Wskazówka z dokumentacji Google



Dokumentacja Google AI zaleca używanie konkretnych i zróżnicowanych przykładów w promptach typu few-shot, aby dokładniej określić oczekiwany wynik.



Źródło: https://ai.google.dev/gemini-api/docs/prompting-strategies



Data zapoznania się: 31.10.2026.



Wskazówkę wykorzystano w rundzie 4, dodając trzy różne przykłady notatek, które pokazywały agentowi oczekiwany format zapisu.

