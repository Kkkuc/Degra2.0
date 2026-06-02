CREATE OR REPLACE TRIGGER TRG_STUDENTLOG
AFTER INSERT OR UPDATE OR DELETE ON "Students" 
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."StudentID" || ', Imie: ' || :NEW."FirstName" || 
                     ', Nazwisko: ' || :NEW."LastName";
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :NEW."StudentID" || ', Imie: ' || :NEW."FirstName" || 
                     ', Nazwisko: ' || :NEW."LastName";
                     
        v_new_val := 'ID: ' || :NEW."StudentID" || ', Imie: ' || :NEW."FirstName" || 
                     ', Nazwisko: ' || :NEW."LastName";
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :NEW."StudentID" || ', Imie: ' || :NEW."FirstName" || 
                     ', Nazwisko: ' || :NEW."LastName";
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", 
        "Operation", 
        "OldValue", 
        "NewValue", 
        "UserChanged", 
        "ChangedAt"
    ) VALUES (
        'Student', 
        v_operation, 
        v_old_val, 
        v_new_val, 
        v_user, 
        CURRENT_TIMESTAMP
    );
END;

/
CREATE OR REPLACE TRIGGER TRG_TEACHERLOG
AFTER INSERT OR UPDATE OR DELETE ON "Teachers"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
                    
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                        ', Tytul: ' || NVL(:NEW."AcademicTitle", 'Brak') || 
                        ', Imie: ' || :NEW."FirstName" || 
                        ', Nazwisko: ' || :NEW."LastName" || 
                        ', Email: ' || NVL(:NEW."Email", 'Brak');
                                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                        ', Tytul: ' || NVL(:OLD."AcademicTitle", 'Brak') || 
                        ', Imie: ' || :OLD."FirstName" || 
                        ', Nazwisko: ' || :OLD."LastName" || 
                        ', Email: ' || NVL(:OLD."Email", 'Brak');
                                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                        ', Tytul: ' || NVL(:NEW."AcademicTitle", 'Brak') || 
                        ', Imie: ' || :NEW."FirstName" || 
                        ', Nazwisko: ' || :NEW."LastName" || 
                        ', Email: ' || NVL(:NEW."Email", 'Brak');
                                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                        ', Tytul: ' || NVL(:OLD."AcademicTitle", 'Brak') || 
                        ', Imie: ' || :OLD."FirstName" || 
                        ', Nazwisko: ' || :OLD."LastName" || 
                        ', Email: ' || NVL(:OLD."Email", 'Brak');
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'Teacher', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/

CREATE OR REPLACE TRIGGER TRG_TIMETABLELOG
AFTER INSERT OR UPDATE OR DELETE ON "Timetables"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);

    FUNCTION ParseClassType(p_val IN NUMBER) RETURN NVARCHAR2 IS
    BEGIN

        RETURN CASE p_val
            WHEN 0 THEN 'Lecture'
            WHEN 1 THEN 'Laboratory'
            WHEN 2 THEN 'SpecialisedLaboratory'
            WHEN 3 THEN 'Exercise'
            WHEN 3 THEN 'Seminar'
            WHEN 3 THEN 'Project'
            ELSE 'Nieznany typ (' || TO_CHAR(p_val) || ')'
        END;
    END;

    FUNCTION ParseWeekCycle(p_val IN NUMBER) RETURN NVARCHAR2 IS
    BEGIN
        RETURN CASE p_val
            WHEN 0 THEN 'Weekly'
            WHEN 1 THEN 'Even'
            WHEN 2 THEN 'Odd'
            ELSE 'Nieznany cykl (' || TO_CHAR(p_val) || ')'
        END;
    END;

BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', PrzedmiotID: ' || :NEW."SubjectId" || 
                     ', NauczycielID: ' || :NEW."TeacherId" || 
                     ', SalaID: ' || :NEW."RoomId" || 
                     ', GrupaID: ' || :NEW."GroupId" || 
                     ', TypZajec: ' || ParseClassType(:NEW."ClassType") || 
                     ', Dzien: ' || :NEW."DayOfWeek" || 
                     ', Start: ' || :NEW."StartTime" || 
                     ', Koniec: ' || :NEW."EndTime" || 
                     ', Cykl: ' || ParseWeekCycle(:NEW."WeekCycle");
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', PrzedmiotID: ' || :OLD."SubjectId" || 
                     ', NauczycielID: ' || :OLD."TeacherId" || 
                     ', SalaID: ' || :OLD."RoomId" || 
                     ', GrupaID: ' || :OLD."GroupId" || 
                     ', TypZajec: ' || ParseClassType(:OLD."ClassType") || 
                     ', Dzien: ' || :OLD."DayOfWeek" || 
                     ', Start: ' || :OLD."StartTime" || 
                     ', Koniec: ' || :OLD."EndTime" || 
                     ', Cykl: ' || ParseWeekCycle(:OLD."WeekCycle");
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', PrzedmiotID: ' || :NEW."SubjectId" || 
                     ', NauczycielID: ' || :NEW."TeacherId" || 
                     ', SalaID: ' || :NEW."RoomId" || 
                     ', GrupaID: ' || :NEW."GroupId" || 
                     ', TypZajec: ' || ParseClassType(:NEW."ClassType") || 
                     ', Dzien: ' || :NEW."DayOfWeek" || 
                     ', Start: ' || :NEW."StartTime" || 
                     ', Koniec: ' || :NEW."EndTime" || 
                     ', Cykl: ' || ParseWeekCycle(:NEW."WeekCycle");
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', PrzedmiotID: ' || :OLD."SubjectId" || 
                     ', NauczycielID: ' || :OLD."TeacherId" || 
                     ', SalaID: ' || :OLD."RoomId" || 
                     ', GrupaID: ' || :OLD."GroupId" || 
                     ', TypZajec: ' || ParseClassType(:OLD."ClassType") || 
                     ', Dzien: ' || :OLD."DayOfWeek" || 
                     ', Start: ' || :OLD."StartTime" || 
                     ', Koniec: ' || :OLD."EndTime" || 
                     ', Cykl: ' || ParseWeekCycle(:OLD."WeekCycle");
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'Timetables', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/

CREATE OR REPLACE TRIGGER TRG_SUBJECTLOG
AFTER INSERT OR UPDATE OR DELETE ON "Subjects"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Skrot: ' || NVL(:NEW."Abbreviation", 'Brak') || 
                     ', Kod: ' || NVL(:NEW."Code", 'Brak');
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Skrot: ' || NVL(:OLD."Abbreviation", 'Brak') || 
                     ', Kod: ' || NVL(:OLD."Code", 'Brak');
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Skrot: ' || NVL(:NEW."Abbreviation", 'Brak') || 
                     ', Kod: ' || NVL(:NEW."Code", 'Brak');
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Skrot: ' || NVL(:OLD."Abbreviation", 'Brak') || 
                     ', Kod: ' || NVL(:OLD."Code", 'Brak');
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'Subjects', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/

CREATE OR REPLACE TRIGGER TRG_SEMESTRLOG
AFTER INSERT OR UPDATE OR DELETE ON "Semesters" 
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', RokAkademickiID: ' || :NEW."AcademicYearId" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Start: ' || TO_CHAR(:NEW."StartDate", 'YYYY-MM-DD') || 
                     ', Koniec: ' || TO_CHAR(:NEW."EndDate", 'YYYY-MM-DD');
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', RokAkademickiID: ' || :OLD."AcademicYearId" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Start: ' || TO_CHAR(:OLD."StartDate", 'YYYY-MM-DD') || 
                     ', Koniec: ' || TO_CHAR(:OLD."EndDate", 'YYYY-MM-DD');
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', RokAkademickiID: ' || :NEW."AcademicYearId" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Start: ' || TO_CHAR(:NEW."StartDate", 'YYYY-MM-DD') || 
                     ', Koniec: ' || TO_CHAR(:NEW."EndDate", 'YYYY-MM-DD');
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', RokAkademickiID: ' || :OLD."AcademicYearId" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Start: ' || TO_CHAR(:OLD."StartDate", 'YYYY-MM-DD') || 
                     ', Koniec: ' || TO_CHAR(:OLD."EndDate", 'YYYY-MM-DD');
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'Semesters', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_USERLOG
AFTER INSERT OR UPDATE OR DELETE ON "Users"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Login: ' || :NEW."Username" || 
                     ', Email: ' || :NEW."Email" || 
                     ', RoleID: ' || :NEW."RoleId" || 
                     ', StudentID: ' || NVL(:NEW."StudentId", 'Brak') || 
                     ', TeacherID: ' || NVL(TO_CHAR(:NEW."TeacherId"), 'Brak') || 
                     ', Aktywny: ' || NVL(TO_CHAR(:NEW."IsActive"), 'Brak') || 
                     ', OstatnieLog: ' || NVL(TO_CHAR(:NEW."LastLogin", 'YYYY-MM-DD HH24:MI'), 'Brak') || 
                     ', Haslo: [UKRYTE]';
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Login: ' || :OLD."Username" || 
                     ', Email: ' || :OLD."Email" || 
                     ', RoleID: ' || :OLD."RoleId" || 
                     ', StudentID: ' || NVL(:OLD."StudentId", 'Brak') || 
                     ', TeacherID: ' || NVL(TO_CHAR(:OLD."TeacherId"), 'Brak') || 
                     ', Aktywny: ' || NVL(TO_CHAR(:OLD."IsActive"), 'Brak') || 
                     ', OstatnieLog: ' || NVL(TO_CHAR(:OLD."LastLogin", 'YYYY-MM-DD HH24:MI'), 'Brak') || 
                     ', Haslo: [UKRYTE]';
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Login: ' || :NEW."Username" || 
                     ', Email: ' || :NEW."Email" || 
                     ', RoleID: ' || :NEW."RoleId" || 
                     ', StudentID: ' || NVL(:NEW."StudentId", 'Brak') || 
                     ', TeacherID: ' || NVL(TO_CHAR(:NEW."TeacherId"), 'Brak') || 
                     ', Aktywny: ' || NVL(TO_CHAR(:NEW."IsActive"), 'Brak') || 
                     ', OstatnieLog: ' || NVL(TO_CHAR(:NEW."LastLogin", 'YYYY-MM-DD HH24:MI'), 'Brak') || 
                     ', Haslo: [UKRYTE]';
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Login: ' || :OLD."Username" || 
                     ', Email: ' || :OLD."Email" || 
                     ', RoleID: ' || :OLD."RoleId" || 
                     ', StudentID: ' || NVL(:OLD."StudentId", 'Brak') || 
                     ', TeacherID: ' || NVL(TO_CHAR(:OLD."TeacherId"), 'Brak') || 
                     ', Aktywny: ' || NVL(TO_CHAR(:OLD."IsActive"), 'Brak') || 
                     ', OstatnieLog: ' || NVL(TO_CHAR(:OLD."LastLogin", 'YYYY-MM-DD HH24:MI'), 'Brak') || 
                     ', Haslo: [UKRYTE]';
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'User', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_ACADEMICYEARLOG
AFTER INSERT OR UPDATE OR DELETE ON "AcademicYears"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Start: ' || TO_CHAR(:NEW."StartDate", 'YYYY-MM-DD') || 
                     ', Koniec: ' || TO_CHAR(:NEW."EndDate", 'YYYY-MM-DD');
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Start: ' || TO_CHAR(:OLD."StartDate", 'YYYY-MM-DD') || 
                     ', Koniec: ' || TO_CHAR(:OLD."EndDate", 'YYYY-MM-DD');
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Start: ' || TO_CHAR(:NEW."StartDate", 'YYYY-MM-DD') || 
                     ', Koniec: ' || TO_CHAR(:NEW."EndDate", 'YYYY-MM-DD');
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Start: ' || TO_CHAR(:OLD."StartDate", 'YYYY-MM-DD') || 
                     ', Koniec: ' || TO_CHAR(:OLD."EndDate", 'YYYY-MM-DD');
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'AcademicYear', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_BUILDINGLOG
AFTER INSERT OR UPDATE OR DELETE ON "Buildings"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Ulica: ' || :NEW."Street" || 
                     ', NrDomu: ' || :NEW."HouseNumber" || 
                     ', Miasto: ' || :NEW."City" || 
                     ', KodPocztowy: ' || :NEW."PostalCode" || 
                     ', WydzialID: ' || :NEW."FacultyId";
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Ulica: ' || :OLD."Street" || 
                     ', NrDomu: ' || :OLD."HouseNumber" || 
                     ', Miasto: ' || :OLD."City" || 
                     ', KodPocztowy: ' || :OLD."PostalCode" || 
                     ', WydzialID: ' || :OLD."FacultyId";
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Ulica: ' || :NEW."Street" || 
                     ', NrDomu: ' || :NEW."HouseNumber" || 
                     ', Miasto: ' || :NEW."City" || 
                     ', KodPocztowy: ' || :NEW."PostalCode" || 
                     ', WydzialID: ' || :NEW."FacultyId";
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Ulica: ' || :OLD."Street" || 
                     ', NrDomu: ' || :OLD."HouseNumber" || 
                     ', Miasto: ' || :OLD."City" || 
                     ', KodPocztowy: ' || :OLD."PostalCode" || 
                     ', WydzialID: ' || :OLD."FacultyId";
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'Building', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_FACULTYLOG
AFTER INSERT OR UPDATE OR DELETE ON "Faculties" 
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Skrot: ' || NVL(:NEW."Abbreviation", 'Brak');
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Skrot: ' || NVL(:OLD."Abbreviation", 'Brak');
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Skrot: ' || NVL(:NEW."Abbreviation", 'Brak');
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Skrot: ' || NVL(:OLD."Abbreviation", 'Brak');
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'Faculty', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_FIELDOFSTUDYLOG
AFTER INSERT OR UPDATE OR DELETE ON "FieldsOfStudy" -- Uwaga: sprawdź, czy EF nie wygenerował np. "FieldsOfStudy"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', WydzialID: ' || :NEW."FacultyId" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Stopien: ' || NVL(:NEW."Degree", 'Brak') || 
                     ', Tryb: ' || NVL(:NEW."Mode", 'Brak');
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', WydzialID: ' || :OLD."FacultyId" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Stopien: ' || NVL(:OLD."Degree", 'Brak') || 
                     ', Tryb: ' || NVL(:OLD."Mode", 'Brak');
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', WydzialID: ' || :NEW."FacultyId" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Stopien: ' || NVL(:NEW."Degree", 'Brak') || 
                     ', Tryb: ' || NVL(:NEW."Mode", 'Brak');
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', WydzialID: ' || :OLD."FacultyId" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Stopien: ' || NVL(:OLD."Degree", 'Brak') || 
                     ', Tryb: ' || NVL(:OLD."Mode", 'Brak');
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'FieldOfStudy', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_GROUPLOG
AFTER INSERT OR UPDATE OR DELETE ON "Groups" -- Sprawdź, czy nie "Groups"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);

    -- Funkcja do parsowania ClassType (taka sama jak w Timetable)
    FUNCTION ParseClassType(p_val IN NUMBER) RETURN NVARCHAR2 IS
    BEGIN
        -- TUTAJ PODMIEŃ WARTOŚCI NA SWOJE Z C# (jeśli są inne)
        RETURN CASE p_val
            WHEN 0 THEN 'Wyklad'
            WHEN 1 THEN 'Cwiczenia'
            WHEN 2 THEN 'Laboratorium'
            WHEN 3 THEN 'Seminarium'
            ELSE 'Nieznany typ (' || TO_CHAR(p_val) || ')'
        END;
    END;

BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', SemestrID: ' || :NEW."SemesterId" || 
                     ', KierunekID: ' || :NEW."FieldOfStudyId" || 
                     ', SpecjalizacjaID: ' || NVL(TO_CHAR(:NEW."SpecializationId"), 'Brak') || 
                     ', TypZajec: ' || ParseClassType(:NEW."ClassType") || 
                     ', Nazwa: ' || :NEW."Name";
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', SemestrID: ' || :OLD."SemesterId" || 
                     ', KierunekID: ' || :OLD."FieldOfStudyId" || 
                     ', SpecjalizacjaID: ' || NVL(TO_CHAR(:OLD."SpecializationId"), 'Brak') || 
                     ', TypZajec: ' || ParseClassType(:OLD."ClassType") || 
                     ', Nazwa: ' || :OLD."Name";
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', SemestrID: ' || :NEW."SemesterId" || 
                     ', KierunekID: ' || :NEW."FieldOfStudyId" || 
                     ', SpecjalizacjaID: ' || NVL(TO_CHAR(:NEW."SpecializationId"), 'Brak') || 
                     ', TypZajec: ' || ParseClassType(:NEW."ClassType") || 
                     ', Nazwa: ' || :NEW."Name";
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', SemestrID: ' || :OLD."SemesterId" || 
                     ', KierunekID: ' || :OLD."FieldOfStudyId" || 
                     ', SpecjalizacjaID: ' || NVL(TO_CHAR(:OLD."SpecializationId"), 'Brak') || 
                     ', TypZajec: ' || ParseClassType(:OLD."ClassType") || 
                     ', Nazwa: ' || :OLD."Name";
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'Group', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_ROLELOG
AFTER INSERT OR UPDATE OR DELETE ON "Roles"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name";
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name";
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Nazwa: ' || :NEW."Name";
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Nazwa: ' || :OLD."Name";
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'Role', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_ROLEPERMISSONLOG
AFTER INSERT OR UPDATE OR DELETE ON "RolePermissions" -- Sprawdź, czy EF nie wygenerował nazwy "RolePermissions"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'RoleID: ' || :NEW."RoleId" || 
                     ', PermissionID: ' || :NEW."PermissionId";
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'RoleID: ' || :OLD."RoleId" || 
                     ', PermissionID: ' || :OLD."PermissionId";
                     
        v_new_val := 'RoleID: ' || :NEW."RoleId" || 
                     ', PermissionID: ' || :NEW."PermissionId";
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'RoleID: ' || :OLD."RoleId" || 
                     ', PermissionID: ' || :OLD."PermissionId";
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'RolePermission', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_PERMISSIONLOG
AFTER INSERT OR UPDATE OR DELETE ON "Permissions"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
    v_user NVARCHAR2(200);
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Kod: ' || :NEW."PermissionCode" || 
                     ', Opis: ' || NVL(:NEW."Description", 'Brak');
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Kod: ' || :OLD."PermissionCode" || 
                     ', Opis: ' || NVL(:OLD."Description", 'Brak');
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', Kod: ' || :NEW."PermissionCode" || 
                     ', Opis: ' || NVL(:NEW."Description", 'Brak');
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', Kod: ' || :OLD."PermissionCode" || 
                     ', Opis: ' || NVL(:OLD."Description", 'Brak');
    END IF;

    INSERT INTO "Logs" ( 
        "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
    ) VALUES (
        'Permission', v_operation, v_old_val, v_new_val, v_user, CURRENT_TIMESTAMP
    );
END;
/