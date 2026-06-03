DECLARE
    TYPE t_names IS TABLE OF VARCHAR2(100);
    v_tables t_names := t_names('AcademicYears', 'Buildings', 'Faculties', 'FieldsOfStudy', 'Groups', 'Logs', 'Permissions', 'RolePermissions', 'Roles', 'Rooms', 'ScheduleChanges', 'Semesters', 'Specializations', 'StudentGroups', 'Students', 'Subjects', 'Teachers', 'Timetables');
    v_real_name VARCHAR2(100);
BEGIN
    FOR i IN 1..v_tables.COUNT LOOP
            BEGIN
                SELECT table_name INTO v_real_name FROM user_tables WHERE UPPER(table_name) = UPPER(v_tables(i));
                FOR c IN (SELECT constraint_name FROM user_constraints WHERE table_name = v_real_name AND constraint_type = 'R') LOOP
                        EXECUTE IMMEDIATE 'ALTER TABLE "' || v_real_name || '" DISABLE CONSTRAINT "' || c.constraint_name || '"';
                    END LOOP;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN NULL;
            END;
        END LOOP;

    FOR i IN 1..v_tables.COUNT LOOP
            BEGIN
                SELECT table_name INTO v_real_name FROM user_tables WHERE UPPER(table_name) = UPPER(v_tables(i));
                EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || v_real_name || '"';
            EXCEPTION
                WHEN NO_DATA_FOUND THEN NULL;
            END;
        END LOOP;

    FOR i IN 1..v_tables.COUNT LOOP
            BEGIN
                SELECT table_name INTO v_real_name FROM user_tables WHERE UPPER(table_name) = UPPER(v_tables(i));
                FOR c IN (SELECT constraint_name FROM user_constraints WHERE table_name = v_real_name AND constraint_type = 'R') LOOP
                        EXECUTE IMMEDIATE 'ALTER TABLE "' || v_real_name || '" ENABLE CONSTRAINT "' || c.constraint_name || '"';
                    END LOOP;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN NULL;
            END;
        END LOOP;
END;
/

TRUNCATE TABLE "AcademicYears" CASCADE;