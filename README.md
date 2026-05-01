# 🐳 Jak postawić bazę ORACLE

## 1. Instalacja Dockera

Zainstaluj Dockera, najlepiej **Docker Desktop**.

---

## 2. Uruchomienie bazy Oracle w projekcie

W projekcie uruchom taki skrypt w terminalu:

```bash
docker run -d --name LocalHostDegra -p 1521:1521 -e ORACLE_PWD=mikus_haslo container-registry.oracle.com/database/free:latest
```

---

## 3. Zmiana konfiguracji aplikacji

Zmień chwilowo w `appsettings.json`:

```
User Id: DEGRA_ADMIN → SYSTEM
```

---

## 4. Połączenie z bazą Oracle (Rider / środowisko) podpowiedz chata wierzę że nie potrzebujecie ale umieszczam

Oto co musisz zmienić w tym oknie:

### 1. 📥 Pobierz sterowniki (Driver)

Zauważ napis **"Oracle Not downloaded"** obok ikony Oracle.

Kliknij niebieski napis **Download** po prawej stronie. Rider sam pobierze potrzebne pliki JAR, aby móc połączyć się z bazą. Bez tego przycisk **"Test Connection"** nie zadziała.

---

### 2. 🔄 Zmień SID na Service Name

Wersja Oracle 23c Free (ta z Dockera) używa domyślnie **Service Name** zamiast SID.

- Zmień `Connection type` z **SID** na **Service Name**
- W polu, gdzie teraz masz wpisane `XE`, wpisz:

```
FREEPDB1
```

(To bardzo ważne – FREEPDB1 to domyślna nazwa bazy użytkownika w tym obrazie Dockera. XE było używane w starszych wersjach)

---

### 3. 🔐 Zweryfikuj User i Password

- **User:** SYSTEM (dużymi literami)
- **Password:** mikus_haslo (to samo co w docker run)

---

### 4. 🧪 Jak sprawdzić czy działa "Test Connection"?

Upewnij się że:

- kontener działa (`Running`)
- wpisz:

```bash
docker ps
```

Jeśli chcesz:

```bash
docker logs oracle23c
```

Jeśli widzisz:

```
DATABASE IS READY TO USE!
```

kliknij **Test Connection**.

koniec chata

---

## 5. 🧱 Utworzenie użytkownika bazy

W konsoli SQL (console query):

```sql
CREATE USER DEGRA_ADMIN IDENTIFIED BY mikus_haslo;
GRANT CONNECT, RESOURCE, DBA TO DEGRA_ADMIN;
ALTER USER DEGRA_ADMIN QUOTA UNLIMITED ON USERS;
```

---

## 6. 🔁 Finalna zmiana konfiguracji

Wróć do:

```
DEGRA_ADMIN
```

i uruchom:

```bash
dotnet ef database update
```

---

## 7. 💬 Uwagi końcowe

Jeśli chcemy zrobić to tak, żeby każdy miał wspólną bazę danych, to jest to bezsensu, ale można, trochę dodatkowej roboty :)
