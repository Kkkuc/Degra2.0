CREATE OR REPLACE PACKAGE LOG_Pkg IS
    -- Publiczne funkcje do parsowania Enumów (żeby triggery mogły z nich korzystać)
    FUNCTION ParseClassType(p_val IN NUMBER) RETURN NVARCHAR2;
    FUNCTION ParseRoomType(p_val IN NUMBER) RETURN NVARCHAR2;
    FUNCTION ParseWeekCycle(p_val IN NUMBER) RETURN NVARCHAR2;
    FUNCTION ParseStudyMode(p_val IN NUMBER) RETURN NVARCHAR2;
        
    -- Główna procedura zapisująca log
    PROCEDURE SaveLog(
        p_table_name IN NVARCHAR2,
        p_operation  IN NVARCHAR2,
        p_old_val    IN NVARCHAR2,
        p_new_val    IN NVARCHAR2
    );
END LOG_Pkg;
/
CREATE OR REPLACE PACKAGE BODY LOG_Pkg IS

   FUNCTION ParseClassType(p_val IN NUMBER) RETURN NVARCHAR2 IS
    BEGIN
        RETURN CASE p_val
            WHEN 0 THEN 'Lecture'
            WHEN 1 THEN 'Laboratory'
            WHEN 2 THEN 'SpecialisedLaboratory'
            WHEN 3 THEN 'Exercise'
            WHEN 4 THEN 'Seminar'
            WHEN 5 THEN 'Project'
            ELSE 'Nieznany typ (' || TO_CHAR(p_val) || ')'
        END;
    END ParseClassType;

    FUNCTION ParseRoomType(p_val IN NUMBER) RETURN NVARCHAR2 IS
    BEGIN
        RETURN CASE p_val
            WHEN 0 THEN 'LectureHall'
            WHEN 1 THEN 'Laboratory'
            WHEN 2 THEN 'SeminarRoom'
            WHEN 3 THEN 'ComputerLab'
            WHEN 4 THEN 'Other'
            ELSE 'Nieznany typ (' || TO_CHAR(p_val) || ')'
    END;
    END ParseRoomType;

    FUNCTION ParseWeekCycle(p_val IN NUMBER) RETURN NVARCHAR2 IS
    BEGIN
        RETURN CASE p_val
            WHEN 0 THEN 'Weekly'
            WHEN 1 THEN 'Even'
            WHEN 2 THEN 'Odd'
            ELSE 'Nieznany cykl (' || TO_CHAR(p_val) || ')'
    END;
    END ParseWeekCycle;
       
    FUNCTION ParseStudyMode(p_val IN NUMBER) RETURN NVARCHAR2 IS
    BEGIN 
        RETURN CASE p_val
            WHEN 0 THEN 'FullTime'
            WHEN 1 THEN 'PartTime'
            WHEN 2 THEN 'Postgraduate'
    END;
    END ParseStudyMode;

    PROCEDURE SaveLog(
        p_table_name IN NVARCHAR2,
        p_operation  IN NVARCHAR2,
        p_old_val    IN NVARCHAR2,
        p_new_val    IN NVARCHAR2
    ) IS
        v_user NVARCHAR2(200);
    BEGIN
        v_user := SYS_CONTEXT('USERENV', 'SESSION_USER');
        INSERT INTO "Logs" ( 
            "TableName", "Operation", "OldValue", "NewValue", "UserChanged", "ChangedAt"
        ) VALUES (
            p_table_name, p_operation, p_old_val, p_new_val, v_user, CURRENT_TIMESTAMP
        );
    END SaveLog;

END LOG_Pkg;
/

CREATE OR REPLACE TRIGGER TRG_STUDENT_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Students" 
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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
    LOG_pkg.SaveLog('Students', v_operation, v_old_val, v_new_val);
END;
/
CREATE OR REPLACE TRIGGER TRG_TEACHER_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Teachers"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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
    LOG_pkg.SaveLog('Teachers', v_operation, v_old_val, v_new_val);
END;
/
CREATE OR REPLACE TRIGGER trg_timetable_log
AFTER INSERT OR UPDATE OR DELETE ON "Timetables"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(32767);
    v_new_val NVARCHAR2(32767);

    FUNCTION FormatInterval(p_int INTERVAL DAY TO SECOND) RETURN NVARCHAR2 IS
    BEGIN
        RETURN LPAD(EXTRACT(HOUR FROM p_int), 2, '0') || ':' ||
               LPAD(EXTRACT(MINUTE FROM p_int), 2, '0');
    END;
    
BEGIN
    v_old_val := '-';
    v_new_val := '-';
    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', PrzedmiotID: ' || :NEW."SubjectId" || 
                     ', NauczycielID: ' || :NEW."TeacherId" || 
                     ', SalaID: ' || :NEW."RoomId" || 
                     ', GrupaID: ' || :NEW."GroupId" || 
                     ', TypZajec: ' || :NEW."ClassType" || 
                     ', Dzien: ' || :NEW."DayOfWeek" ||
                     ', Start: ' || FormatInterval(:NEW."StartTime") ||
                     ', Koniec: ' || FormatInterval(:NEW."EndTime") ||
                     ', Cykl: ' || :NEW."WeekCycle";
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', PrzedmiotID: ' || :OLD."SubjectId" || 
                     ', NauczycielID: ' || :OLD."TeacherId" || 
                     ', SalaID: ' || :OLD."RoomId" || 
                     ', GrupaID: ' || :OLD."GroupId" || 
                     ', TypZajec: ' ||:OLD."ClassType" || 
                     ', Dzien: ' || :OLD."DayOfWeek" ||
                     ', Start: ' || FormatInterval(:OLD."StartTime") ||
                     ', Koniec: ' || FormatInterval(:OLD."EndTime") ||
                     ', Cykl: ' || :OLD."WeekCycle";
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', PrzedmiotID: ' || :NEW."SubjectId" || 
                     ', NauczycielID: ' || :NEW."TeacherId" || 
                     ', SalaID: ' || :NEW."RoomId" || 
                     ', GrupaID: ' || :NEW."GroupId" || 
                     ', TypZajec: ' || :NEW."ClassType" || 
                     ', Dzien: ' || :NEW."DayOfWeek" ||
                     ', Start: ' || FormatInterval(:NEW."StartTime") ||
                     ', Koniec: ' || FormatInterval(:NEW."EndTime") ||
                     ', Cykl: ' || :NEW."WeekCycle";
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', PrzedmiotID: ' || :OLD."SubjectId" || 
                     ', NauczycielID: ' || :OLD."TeacherId" || 
                     ', SalaID: ' || :OLD."RoomId" || 
                     ', GrupaID: ' || :OLD."GroupId" || 
                     ', TypZajec: ' || :OLD."ClassType" || 
                     ', Dzien: ' || :OLD."DayOfWeek" ||
                     ', Start: ' || FormatInterval(:OLD."StartTime") ||
                     ', Koniec: ' || FormatInterval(:OLD."EndTime") ||
                     ', Cykl: ' || :OLD."WeekCycle";
    END IF;

    LOG_pkg.SaveLog('TimeTables', v_operation, v_old_val, v_new_val);
END;
/

CREATE OR REPLACE TRIGGER TRG_SUBJECT_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Subjects"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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

     LOG_pkg.SaveLog('Subjects', v_operation, v_old_val, v_new_val);
END;
/

CREATE OR REPLACE TRIGGER TRG_SEMESTR_LOG
    AFTER INSERT OR UPDATE OR DELETE ON "Semesters"
    FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val   NVARCHAR2(2000);
    v_new_val   NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

    IF INSERTING THEN
        v_operation := 'INSERT';

        v_new_val :=
                'ID: ' || :NEW."Id" ||
                ', RokAkademickiID: ' || :NEW."AcademicYearId" ||
                ', Nazwa: ' || :NEW."Name" ||
                ', Start: ' || :NEW."StartDate" ||
                ', Koniec: ' || :NEW."EndDate";

    ELSIF UPDATING THEN
        v_operation := 'UPDATE';

        v_old_val :=
                'ID: ' || :OLD."Id" ||
                ', RokAkademickiID: ' || :OLD."AcademicYearId" ||
                ', Nazwa: ' || :OLD."Name" ||
                ', Start: ' || :OLD."StartDate" ||
                ', Koniec: ' || :OLD."EndDate";

        v_new_val :=
                'ID: ' || :NEW."Id" ||
                ', RokAkademickiID: ' || :NEW."AcademicYearId" ||
                ', Nazwa: ' || :NEW."Name" ||
                ', Start: ' || :NEW."StartDate" ||
                ', Koniec: ' || :NEW."EndDate";

    ELSIF DELETING THEN
        v_operation := 'DELETE';

        v_old_val :=
                'ID: ' || :OLD."Id" ||
                ', RokAkademickiID: ' || :OLD."AcademicYearId" ||
                ', Nazwa: ' || :OLD."Name" ||
                ', Start: ' || :OLD."StartDate" ||
                ', Koniec: ' || :OLD."EndDate";
    END IF;

    LOG_pkg.SaveLog(
            'Semesters',
            v_operation,
            v_old_val,
            v_new_val
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_USER_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Users"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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

    LOG_pkg.SaveLog('Users', v_operation, v_old_val, v_new_val);
END;
/
CREATE OR REPLACE TRIGGER TRG_ACADEMICYEAR_LOG
AFTER INSERT OR UPDATE OR DELETE ON "AcademicYears"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);

BEGIN
    v_old_val := '-';
    v_new_val := '-';
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
    LOG_pkg.SaveLog('AcademicYears', v_operation, v_old_val, v_new_val);
END;
/
CREATE OR REPLACE TRIGGER TRG_BUILDING_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Buildings"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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

    LOG_pkg.SaveLog('Buildings', v_operation, v_old_val, v_new_val);
END;
/
CREATE OR REPLACE TRIGGER TRG_FACULTYLOG_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Faculties" 
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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
    
    LOG_pkg.SaveLog('Faculties', v_operation, v_old_val, v_new_val);
END;
/
CREATE OR REPLACE TRIGGER TRG_FIELD_OF_STUDY_LOG
AFTER INSERT OR UPDATE OR DELETE ON "FieldsOfStudy" 
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', WydzialID: ' || :NEW."FacultyId" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Stopien: ' || NVL(:NEW."Degree", 'Brak') || 
                     ', Tryb: ' || LOG_pkg.ParseClassType(:NEW."Mode");
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', WydzialID: ' || :OLD."FacultyId" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Stopien: ' || NVL(:OLD."Degree", 'Brak') || 
                     ', Tryb: ' || LOG_pkg.ParseClassType(:OLD."Mode");
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', WydzialID: ' || :NEW."FacultyId" || 
                     ', Nazwa: ' || :NEW."Name" || 
                     ', Stopien: ' || NVL(:NEW."Degree", 'Brak') || 
                     ', Tryb: ' || LOG_pkg.ParseClassType(:NEW."Mode");
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', WydzialID: ' || :OLD."FacultyId" || 
                     ', Nazwa: ' || :OLD."Name" || 
                     ', Stopien: ' || NVL(:OLD."Degree", 'Brak') || 
                     ', Tryb: ' || LOG_pkg.ParseClassType(:OLD."Mode");
    END IF;

    LOG_pkg.SaveLog('FieldsOfStudy', v_operation, v_old_val, v_new_val);
END;
/
CREATE OR REPLACE TRIGGER TRG_GROUP_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Groups" 
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', SemestrID: ' || :NEW."SemesterId" || 
                     ', KierunekID: ' || :NEW."FieldOfStudyId" || 
                     ', SpecjalizacjaID: ' || NVL(TO_CHAR(:NEW."SpecializationId"), 'Brak') || 
                     ', TypZajec: ' || LOG_pkg.ParseClassType(:NEW."ClassType") || 
                     ', Nazwa: ' || :NEW."Name";
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', SemestrID: ' || :OLD."SemesterId" || 
                     ', KierunekID: ' || :OLD."FieldOfStudyId" || 
                     ', SpecjalizacjaID: ' || NVL(TO_CHAR(:OLD."SpecializationId"), 'Brak') || 
                     ', TypZajec: ' || LOG_pkg.ParseClassType(:OLD."ClassType") || 
                     ', Nazwa: ' || :OLD."Name";
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', SemestrID: ' || :NEW."SemesterId" || 
                     ', KierunekID: ' || :NEW."FieldOfStudyId" || 
                     ', SpecjalizacjaID: ' || NVL(TO_CHAR(:NEW."SpecializationId"), 'Brak') || 
                     ', TypZajec: ' || LOG_pkg.ParseClassType(:NEW."ClassType") || 
                     ', Nazwa: ' || :NEW."Name";
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', SemestrID: ' || :OLD."SemesterId" || 
                     ', KierunekID: ' || :OLD."FieldOfStudyId" || 
                     ', SpecjalizacjaID: ' || NVL(TO_CHAR(:OLD."SpecializationId"), 'Brak') || 
                     ', TypZajec: ' || LOG_pkg.ParseClassType(:OLD."ClassType") || 
                     ', Nazwa: ' || :OLD."Name";
    END IF;

    LOG_pkg.SaveLog('Groups', v_operation, v_old_val, v_new_val);

END;
/
CREATE OR REPLACE TRIGGER TRG_ROLE_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Roles"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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

    LOG_pkg.SaveLog('Roles', v_operation, v_old_val, v_new_val);

END;
/
CREATE OR REPLACE TRIGGER TRG_ROLE_PERMISSON_LOG
AFTER INSERT OR UPDATE OR DELETE ON "RolePermissions"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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

    LOG_pkg.SaveLog('RolePermissions', v_operation, v_old_val, v_new_val);

END;
/
CREATE OR REPLACE TRIGGER TRG_PERMISSION_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Permissions"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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

    LOG_pkg.SaveLog('Permissions', v_operation, v_old_val, v_new_val);

END;
/
CREATE OR REPLACE TRIGGER TRG_ROOM_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Rooms" 
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', BudynekID: ' || :NEW."BuildingId" || 
                     ', NumerSali: ' || :NEW."RoomNumber" || 
                     ', Pojemnosc: ' || NVL(TO_CHAR(:NEW."Capacity"), 'Brak') || 
                     ', TypSali: ' || NVL(:NEW."RoomType", 'Brak');
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', BudynekID: ' || :OLD."BuildingId" || 
                     ', NumerSali: ' || :OLD."RoomNumber" || 
                     ', Pojemnosc: ' || NVL(TO_CHAR(:OLD."Capacity"), 'Brak') || 
                     ', TypSali: ' || NVL(:OLD."RoomType", 'Brak');
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', BudynekID: ' || :NEW."BuildingId" || 
                     ', NumerSali: ' || :NEW."RoomNumber" || 
                     ', Pojemnosc: ' || NVL(TO_CHAR(:NEW."Capacity"), 'Brak') || 
                     ', TypSali: ' || NVL(:NEW."RoomType", 'Brak');
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', BudynekID: ' || :OLD."BuildingId" || 
                     ', NumerSali: ' || :OLD."RoomNumber" || 
                     ', Pojemnosc: ' || NVL(TO_CHAR(:OLD."Capacity"), 'Brak') || 
                     ', TypSali: ' || NVL(:OLD."RoomType", 'Brak');
    END IF;

    LOG_pkg.SaveLog('Rooms', v_operation, v_old_val, v_new_val);

END;
/
CREATE OR REPLACE TRIGGER TRG_SCHEDULE_CHANGE_LOG
AFTER INSERT OR UPDATE OR DELETE ON "ScheduleChanges"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', PlanID: ' || :NEW."TimetableId" || 
                     ', DataZmiany: ' || NVL(TO_CHAR(:NEW."ChangeDate", 'YYYY-MM-DD'), 'Brak') || 
                     ', NowaSalaID: ' || NVL(TO_CHAR(:NEW."NewRoomId"), 'Brak') || 
                     ', NowyNauczycielID: ' || NVL(TO_CHAR(:NEW."NewTeacherId"), 'Brak') || 
                     ', NowyStart: ' || NVL(TO_CHAR(:NEW."NewStartTime"), 'Brak') || 
                     ', NowyKoniec: ' || NVL(TO_CHAR(:NEW."NewEndTime"), 'Brak');
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', PlanID: ' || :OLD."TimetableId" || 
                     ', DataZmiany: ' || NVL(TO_CHAR(:OLD."ChangeDate", 'YYYY-MM-DD'), 'Brak') || 
                     ', NowaSalaID: ' || NVL(TO_CHAR(:OLD."NewRoomId"), 'Brak') || 
                     ', NowyNauczycielID: ' || NVL(TO_CHAR(:OLD."NewTeacherId"), 'Brak') || 
                     ', NowyStart: ' || NVL(TO_CHAR(:OLD."NewStartTime"), 'Brak') || 
                     ', NowyKoniec: ' || NVL(TO_CHAR(:OLD."NewEndTime"), 'Brak');
                     
        v_new_val := 'ID: ' || :NEW."Id" || 
                     ', PlanID: ' || :NEW."TimetableId" || 
                     ', DataZmiany: ' || NVL(TO_CHAR(:NEW."ChangeDate", 'YYYY-MM-DD'), 'Brak') || 
                     ', NowaSalaID: ' || NVL(TO_CHAR(:NEW."NewRoomId"), 'Brak') || 
                     ', NowyNauczycielID: ' || NVL(TO_CHAR(:NEW."NewTeacherId"), 'Brak') || 
                     ', NowyStart: ' || NVL(TO_CHAR(:NEW."NewStartTime"), 'Brak') || 
                     ', NowyKoniec: ' || NVL(TO_CHAR(:NEW."NewEndTime"), 'Brak');
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'ID: ' || :OLD."Id" || 
                     ', PlanID: ' || :OLD."TimetableId" || 
                     ', DataZmiany: ' || NVL(TO_CHAR(:OLD."ChangeDate", 'YYYY-MM-DD'), 'Brak') || 
                     ', NowaSalaID: ' || NVL(TO_CHAR(:OLD."NewRoomId"), 'Brak') || 
                     ', NowyNauczycielID: ' || NVL(TO_CHAR(:OLD."NewTeacherId"), 'Brak') || 
                     ', NowyStart: ' || NVL(TO_CHAR(:OLD."NewStartTime"), 'Brak') || 
                     ', NowyKoniec: ' || NVL(TO_CHAR(:OLD."NewEndTime"), 'Brak');
    END IF;

    LOG_pkg.SaveLog('ScheduleChanges', v_operation, v_old_val, v_new_val);
END;
/
CREATE OR REPLACE TRIGGER TRG_STUDENTGROUP_LOG
AFTER INSERT OR UPDATE OR DELETE ON "StudentGroups" 
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

    IF INSERTING THEN
        v_operation := 'INSERT';
        v_new_val := 'StudentID: ' || :NEW."StudentId" || 
                     ', GroupID: ' || :NEW."GroupId";
                     
    ELSIF UPDATING THEN
        v_operation := 'UPDATE';
        v_old_val := 'StudentID: ' || :OLD."StudentId" || 
                     ', GroupID: ' || :OLD."GroupId";
                     
        v_new_val := 'StudentID: ' || :NEW."StudentId" || 
                     ', GroupID: ' || :NEW."GroupId";
                     
    ELSIF DELETING THEN
        v_operation := 'DELETE';
        v_old_val := 'StudentID: ' || :OLD."StudentId" || 
                     ', GroupID: ' || :OLD."GroupId";
    END IF;

    LOG_pkg.SaveLog('StudentGroups', v_operation, v_old_val, v_new_val);
END;
/
CREATE OR REPLACE TRIGGER TRG_SPECIALIZATION_LOG
AFTER INSERT OR UPDATE OR DELETE ON "Specializations"
FOR EACH ROW
DECLARE
    v_operation NVARCHAR2(20);
    v_old_val NVARCHAR2(2000);
    v_new_val NVARCHAR2(2000);
BEGIN
    v_old_val := '-';
    v_new_val := '-';

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

    LOG_pkg.SaveLog('Specializations', v_operation, v_old_val, v_new_val);

END;
/

ALTER TABLE "Teachers"
    MODIFY "Id"
        GENERATED ALWAYS AS IDENTITY
            (START WITH LIMIT VALUE);