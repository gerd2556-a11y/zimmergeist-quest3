# Zimmergeist: Offline-KI-Haustier für Meta Quest 3

Ein barrierefreies, offline-fähiges Mixed-Reality-Spiel für die **Meta Quest 3**, bei dem ein sanftes Fantasiehaustier („Lumi“) im echten Raum schwebt und auf Gesten, Controller und deutsche Befehle reagiert.

---

## 1. Technischer Status & Bestandsaufnahme

Im System wurden folgende Werkzeuge und Voraussetzungen geprüft:
- **Betriebssystem**: Windows 11 (x64)
- **SideQuest**: Installiert (`1.4.1` unter `AppData\Local\Programs\SideQuest\SideQuest.exe`)
- **Android SDK & ADB**: Installiert (`AppData\Local\Android\Sdk\platform-tools\adb.exe`)
- **Java**: Eclipse Adoptium JDK 21 & JDK 17 vorhanden
- **Unity Hub**: Installiert unter `C:\Program Files\Unity Hub\Unity Hub.exe`
- **Unity Editor**: **Noch nicht vollständig installiert / registriert**. *(Eine unvollständige Installation ohne Unity.exe lag in 6000.3.13f1).*

---

## 2. Installationsschritte in Unity Hub (für den Spieler)

1. Öffne das bereits auf deinem PC installierte **Unity Hub**.
2. Klicke links auf **Installs** (Installationen) -> **Install Editor**.
3. Wähle die empfohlene Version **Unity 6 (6000.0 LTS)**.
4. Setze im Modul-Auswahldialog zwingend ein Häkchen bei:
   - **Android Build Support**
   - **Android SDK & NDK Tools**
   - **OpenJDK**
5. Nach Abschluss der Installation:
   - Klicke im Unity Hub auf **Projects** -> **Add** -> **Add project from disk**.
   - Wähle diesen Ordner aus: `C:\Users\Domenick13\Desktop\vr game`.
   - Öffne das Projekt. Unity lädt und kompiliert die vorkonfigurierten OpenXR- und Haustier-Pakete automatisch.

---

## 3. Architektur & Spielkonzept

### Haustier „Lumi“
- **Gestalt**: Eine schwebende, sanft leuchtende Fantasiekreatur (Himmelskern, animierte Kulleraugen, feine Fühler).
- **Warum ideal für Mixed Reality?**: 
  - Schwebt ruhig im Raum, benötigt keine unzuverlässige Bodenerkennung oder Raumvermessung.
  - Verdeckt reale Gegenstände im Zimmer minimal.
  - Berechenbare, langsame Bewegungen ohne plötzliche Sprünge.

### Barrierefreiheit & Sicherheit
- **Sitz- & Stehmodus**: Kann im Menü jederzeit umgeschaltet werden.
- **Neuausrichtung (Recenter)**: Ein Knopfdruck platziert Lumi sicher in 1,1 m Distanz direkt im Blickfeld.
- **Guardian / Sicherheitsgrenze**: Das Spiel fordert zu keinem Verlassen des sicheren Bereichs auf.

### Offline-Architektur & Sprachbefehle
- **Keine Cloud-Abhängigkeit**: Alle Dialoge und Reaktionen laufen über `LocalPetDialogueService.cs`.
- **Ehrliche Spracherkennung**: Reine On-Device-Spracherkennung im Standard-Build würde ein großes lokales Modell (z. B. Whisper ONNX) voraussetzen. Daher bietet das Spiel eine transparente, vollwertige **Fallback-Befehlsleiste** im UI für alle 8 deutschen Befehle (*Hallo, Füttern, Spielen, Ruh dich aus, Freu dich, Stopp, Menü, Pause*).
- **Datenschutz**: Das Mikrofon lauscht niemals im Hintergrund und sendet keine Audiodaten ins Internet.

---

## 4. APK bauen

Sobald Unity 6 mit Android-Modul installiert ist:
- **Option A (Automatisiert via PowerShell)**:
  ```powershell
  cd "C:\Users\Domenick13\Desktop\vr game\Builds\Quest3"
  .\build_apk.ps1
  ```
- **Option B (Aus dem Unity Editor)**:
  - Öffne das Projekt in Unity.
  - Klicke in der Menüleiste oben auf `Zimmergeist` -> `Build Quest 3 APK`.
  - Die fertige APK wird nach `Builds/Quest3/Zimmergeist.apk` ausgegeben.

---

## 5. Installation mit SideQuest auf die Quest 3

1. **Entwicklermodus auf der Quest 3 aktivieren**:
   - Öffne die Meta Horizon App auf deinem Smartphone.
   - Wähle deine gekoppelte Meta Quest 3 aus -> *Geräteeinstellungen* -> *Entwicklermodus* / *Entwickleroptionen* aktivieren.
2. **Quest per USB-Kabel verbinden**:
   - Schließe das Headset per USB-C an den PC an.
   - Setze die Quest auf und bestätige im Headset den Dialog: *„USB-Debugging zulassen?“* (Häkchen bei *Immer von diesem Computer zulassen* setzen).
3. **SideQuest starten**:
   - Starte SideQuest auf deinem PC.
   - Oben links muss der Punkt **grün** leuchten (z. B. *Quest 3 Connected*).
4. **APK installieren**:
   - Klicke oben rechts in SideQuest auf das Symbol **„Install APK file from folder on computer“** (Symbol mit Pfeil in ein Paket).
   - Wähle die Datei `c:\Users\Domenick13\Desktop\vr game\Builds\Quest3\Zimmergeist.apk` aus.
   - Unten meldet SideQuest: *All tasks completed successfully*.
5. **Spiel auf der Quest 3 starten**:
   - Setze die Quest 3 auf.
   - Öffne deine App-Bibliothek -> Filter oben rechts von *Alle* auf **Unbekannte Quellen** (Unknown Sources) umstellen.
   - Klicke auf **Zimmergeist** und genieße dein Offline-Haustier im eigenen Zimmer!
