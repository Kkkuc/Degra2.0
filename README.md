Jak postawić bazę ORACLE:
# 🚀 Instrukcja konfiguracji bazy danych ORACLE (LocalHostDegra)

Ten dokument opisuje proces stawiania lokalnej instancji bazy danych oraz konfiguracji środowiska dla projektu Degra 2.0.

---

### 1. Przygotowanie środowiska
*   **Zainstaluj dockera**, najlepiej docker desktop.
*   W projekcie uruchom taki skrypt w terminalu:
    
```bash
    docker run -d --name LocalHostDegra -p 1521:1521 -e ORACLE_PWD=mikus_haslo [container-registry.oracle.com/database/free:latest](https://container-registry.oracle.com/database/free:latest)
    ```
*   **Zmień chwilowo** w `appsettings.json` `User Id` z `DEGRA_ADMIN` na `SYSTEM`.

---

### 2. Konfiguracja połączenia w IDE
Następnie połącz się z bazą oracle za pomocą środowiska:

#### 🛠️ Co musisz zmienić w oknie konfiguracji:

1. **Pobierz sterowniki (Driver)**
   Zauważ napis "Oracle Not downloaded" obok ikony Oracle. Kliknij niebieski napis **Download** po prawej stronie. Rider sam pobierze potrzebne pliki JAR, aby móc połączyć się z bazą. Bez tego przycisk "Test Connection" nie zadziała.

2. **Zmień SID na Service Name**
   Wersja Oracle 23c Free (ta z Dockera) używa domyślnie Service Name zamiast SID.
   *   Zmień **Connection type** z SID na **Service Name**.
   *   W polu, gdzie teraz masz wpisane `XE`, wpisz: `FREEPDB1`.
   *(To bardzo ważne – FREEPDB1 to domyślna nazwa bazy użytkownika w tym obrazie Dockera. XE było używane w starszych wersjach).*

3. **Zweryfikuj User i Password**
   *   **User:** Masz `SYSTEM` (dużymi literami) – to jest OK.
   *   **Password:** Upewnij się, że wpisałeś tam dokładnie to samo hasło, które podałeś w komendzie docker run (czyli `mikus_haslo`).

---

### 3. Weryfikacja stanu kontenera
Zanim klikniesz **Test Connection**, upewnij się, że kontener w Dockerze jest w stanie "Running" i przeszedł etap inicjalizacji.

1. Wróć na chwilę do terminala i wpisz: `docker ps`. Powinieneś widzieć swój kontener `LocalHostDegra`.
2. Jeśli chcesz mieć 100% pewności, wpisz: `docker logs LocalHostDegra`.
3. Jeśli widzisz w logach tekst **DATABASE IS READY TO USE!**, wróć do Ridera i kliknij **Test Connection**.

---

### 4. Tworzenie użytkownika dedykowanego
Odpal wtedy **console query** (gdzieś w środowisku powinna być) i wklej oraz uruchom powyższe polecenie (pozwoli to stawiać pliki w folderze użytkownika a nie admina, trochę porządku i tabele nie będą się mierzać):
```sql
CREATE USER DEGRA_ADMIN IDENTIFIED BY mikus_haslo;
GRANT CONNECT, RESOURCE, DBA TO DEGRA_ADMIN;
ALTER USER DEGRA_ADMIN QUOTA UNLIMITED ON USERS;

Teraz możesz w konfiguracji bazy danych i appsetting zmienić użytkownika na DEGRA_ADMIN i możesz wczytać migracje:

'''bash
dotnet ef database update
'''


/////////////////////////////////////////////////////////////////////////////////////////////////////

Zainstaluj dockera, najlepiej docker desktop

W projekcie uruchom taki skrypt w terminalu:

docker run -d --name LocalHostDegra -p 1521:1521 -e ORACLE_PWD=mikus_haslo container-registry.oracle.com/database/free:latest

Zmień chwilowo  w appsettings.json User Id z DEGRA_ADMIN na SYSTEM

Następnie połącz się z bazą oracle za pomocą środowiska (co mi wypluł chat):

////////////////////////////////////////////////////////////////////////////////
Oto co musisz zmienić w tym oknie:

1. Pobierz sterowniki (Driver)
Zauważ napis "Oracle Not downloaded" obok ikony Oracle.

Kliknij niebieski napis Download po prawej stronie. Rider sam pobierze potrzebne pliki JAR, aby móc połączyć się z bazą. Bez tego przycisk "Test Connection" nie zadziała.

2. Zmień SID na Service Name
Wersja Oracle 23c Free (ta z Dockera) używa domyślnie Service Name zamiast SID.

Zmień Connection type z SID na Service Name.

W polu, gdzie teraz masz wpisane XE, wpisz: FREEPDB1.

(To bardzo ważne – FREEPDB1 to domyślna nazwa bazy użytkownika w tym obrazie Dockera. XE było używane w starszych wersjach).

3. Zweryfikuj User i Password
User: Masz SYSTEM (dużymi literami) – to jest OK.

Password: Upewnij się, że wpisałeś tam dokładnie to samo hasło, które podałeś w komendzie docker run (czyli mikus_haslo).

Jak sprawdzić, czy już można klikać "Test Connection"?
Zanim klikniesz Test Connection, upewnij się, że kontener w Dockerze jest w stanie "Running" i przeszedł etap inicjalizacji.

Wróć na chwilę do terminala i wpisz: docker ps. Powinieneś widzieć swój kontener oracle23c.

Jeśli chcesz mieć 100% pewności, wpisz: docker logs oracle23c.

Jeśli widzisz w logach tekst DATABASE IS READY TO USE!, wróć do Ridera i kliknij Test Connection.
///////////////////////////////////////////////////////////////////////////////////////////

Odpal wtedy console query (gdzieś w środowisku powinna być) i wklej oraz uruchom powyższe polecenie (pozwoli to stawiać pliki w folderze użytkownika a nie admina, trochę porządku i tabele nie będą się mieszać)

CREATE USER DEGRA_ADMIN IDENTIFIED BY mikus_haslo;
GRANT CONNECT, RESOURCE, DBA TO DEGRA_ADMIN;
ALTER USER DEGRA_ADMIN QUOTA UNLIMITED ON USERS;


Teraz możesz w konfiguracji bazy danych i appsetting zmienić użytkownika na DEGRA_ADMIN i możesz wczytać migracje

dotnet ef database update

Jeśli chcemy zrobić to żeby każdy miał wspólną bazę danych to to jest bezsensu ale można
