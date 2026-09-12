-- SQL Server setup and sample data for MedicalAppointmentSystemDB.
-- Run this script after selecting the target database.

IF OBJECT_ID(N'dbo.Department', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Department
    (
        DepartmentId INT IDENTITY(1, 1) NOT NULL,
        DepartmentName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(250) NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Department_CreatedDate DEFAULT SYSUTCDATETIME(),
        IsActive BIT NOT NULL CONSTRAINT DF_Department_IsActive DEFAULT 1,
        CONSTRAINT PK_Department PRIMARY KEY (DepartmentId)
    );
END;
GO

IF OBJECT_ID(N'dbo.Patients', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Patients
    (
        Patient_Id INT IDENTITY(1, 1) NOT NULL,
        Full_Name NVARCHAR(100) NOT NULL,
        DateOfBirth DATETIME2 NULL,
        Gender NVARCHAR(50) NOT NULL,
        Contact_Number NVARCHAR(20) NOT NULL,
        Address NVARCHAR(200) NOT NULL,
        CONSTRAINT PK_Patients PRIMARY KEY (Patient_Id)
    );
END;
GO

IF COL_LENGTH(N'dbo.Patients', N'PatientEmail') IS NULL
BEGIN
    ALTER TABLE dbo.Patients ADD PatientEmail NVARCHAR(200) NULL;
END;
ELSE IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Patients')
      AND name = N'PatientEmail'
      AND max_length < 400
)
BEGIN
    ALTER TABLE dbo.Patients ALTER COLUMN PatientEmail NVARCHAR(200) NULL;
END;
GO

IF OBJECT_ID(N'dbo.Doctors', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Doctors
    (
        Doctor_Id INT IDENTITY(1, 1) NOT NULL,
        Full_Name NVARCHAR(100) NOT NULL,
        Specialization NVARCHAR(100) NOT NULL,
        Contact_Number NVARCHAR(20) NOT NULL,
        CONSTRAINT PK_Doctors PRIMARY KEY (Doctor_Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        User_Id INT IDENTITY(1, 1) NOT NULL,
        Full_Name NVARCHAR(100) NOT NULL,
        Mobile NVARCHAR(20) NOT NULL,
        PasswordHash NVARCHAR(100) NOT NULL,
        Role NVARCHAR(30) NOT NULL CONSTRAINT DF_Users_Role DEFAULT N'Patient',
        CONSTRAINT PK_Users PRIMARY KEY (User_Id),
        CONSTRAINT UQ_Users_Mobile UNIQUE (Mobile)
    );
END;
GO

IF OBJECT_ID(N'dbo.Medicines', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Medicines
    (
        Medicine_Id INT IDENTITY(1, 1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(200) NOT NULL,
        CONSTRAINT PK_Medicines PRIMARY KEY (Medicine_Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.Appointments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Appointments
    (
        Appointment_Id INT IDENTITY(1, 1) NOT NULL,
        Patient_Id INT NOT NULL,
        Doctor_Id INT NOT NULL,
        AppointmentDate DATETIME2 NOT NULL,
        Visit_Type NVARCHAR(20) NOT NULL,
        Diagnosis NVARCHAR(500) NOT NULL,
        Notes NVARCHAR(500) NOT NULL,
        CONSTRAINT PK_Appointments PRIMARY KEY (Appointment_Id),
        CONSTRAINT FK_Appointments_Patients FOREIGN KEY (Patient_Id)
            REFERENCES dbo.Patients (Patient_Id),
        CONSTRAINT FK_Appointments_Doctors FOREIGN KEY (Doctor_Id)
            REFERENCES dbo.Doctors (Doctor_Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.PrescriptionDetails', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PrescriptionDetails
    (
        Prescription_Id INT IDENTITY(1, 1) NOT NULL,
        Appointment_Id INT NOT NULL,
        Medicine_Id INT NOT NULL,
        Dosage NVARCHAR(50) NOT NULL,
        Start_Date DATETIME2 NOT NULL,
        End_Date DATETIME2 NOT NULL,
        Notes NVARCHAR(200) NOT NULL,
        CONSTRAINT PK_PrescriptionDetails PRIMARY KEY (Prescription_Id),
        CONSTRAINT FK_PrescriptionDetails_Appointments FOREIGN KEY (Appointment_Id)
            REFERENCES dbo.Appointments (Appointment_Id) ON DELETE CASCADE,
        CONSTRAINT FK_PrescriptionDetails_Medicines FOREIGN KEY (Medicine_Id)
            REFERENCES dbo.Medicines (Medicine_Id)
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_SaveAppointmentWithDetails
    @FullName NVARCHAR(100),
    @DateOfBirth DATETIME2,
    @Gender NVARCHAR(50),
    @ContactNumber NVARCHAR(20),
    @Address NVARCHAR(200),
    @PatientEmail NVARCHAR(200),
    @DepartmentId INT,
    @DoctorId INT,
    @AppointmentDate DATETIME2,
    @Notes NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    DECLARE @PatientId INT;

    SELECT @PatientId = Patient_Id
    FROM dbo.Patients
    WHERE Contact_Number = @ContactNumber;

    IF @PatientId IS NULL
    BEGIN
        INSERT INTO dbo.Patients
            (Full_Name, DateOfBirth, Gender, Contact_Number, Address, PatientEmail)
        VALUES
            (@FullName, @DateOfBirth, @Gender, @ContactNumber, @Address, @PatientEmail);

        SET @PatientId = CONVERT(INT, SCOPE_IDENTITY());
    END;
    ELSE
    BEGIN
        UPDATE dbo.Patients
        SET PatientEmail = COALESCE(NULLIF(@PatientEmail, N''), PatientEmail)
        WHERE Patient_Id = @PatientId;
    END;

    DECLARE @AppointmentId INT;
    INSERT INTO dbo.Appointments
        (Patient_Id, Doctor_Id, AppointmentDate, Visit_Type, Diagnosis, Notes)
    VALUES
        (@PatientId, @DoctorId, @AppointmentDate, N'Consultation',
         N'Appointment requested', COALESCE(@Notes, N''));

    SET @AppointmentId = CONVERT(INT, SCOPE_IDENTITY());

    COMMIT TRANSACTION;

    SELECT @AppointmentId AS AppointmentId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_GetAppointments
    @PageNumber INT,
    @PageSize INT,
    @Role NVARCHAR(30),
    @ContactNumber NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AppointmentCTE AS
    (
        SELECT
            A.Appointment_Id AS AppointmentId,
            A.Patient_Id AS PatientId,
            P.Full_Name AS PatientName,
            CAST(NULL AS NVARCHAR(200)) AS PatientEmail,
            A.Doctor_Id AS DoctorId,
            D.Full_Name AS DoctorName,
            A.AppointmentDate,
            A.Visit_Type AS VisitType,
            A.Diagnosis,
            A.Notes,
            ROW_NUMBER() OVER (ORDER BY A.AppointmentDate DESC, A.Appointment_Id DESC) AS RowNum
        FROM dbo.Appointments AS A
        INNER JOIN dbo.Patients AS P ON A.Patient_Id = P.Patient_Id
        INNER JOIN dbo.Doctors AS D ON A.Doctor_Id = D.Doctor_Id
        WHERE @Role = N'Admin'
           OR P.Contact_Number = @ContactNumber
    )
    SELECT
        AppointmentId,
        PatientId,
        PatientName,
        PatientEmail,
        DoctorId,
        DoctorName,
        AppointmentDate,
        VisitType,
        Diagnosis,
        Notes
    FROM AppointmentCTE
    WHERE RowNum BETWEEN ((@PageNumber - 1) * @PageSize + 1)
                     AND (@PageNumber * @PageSize);

    SELECT COUNT(*) AS TotalCount
    FROM dbo.Appointments AS A
    INNER JOIN dbo.Patients AS P ON A.Patient_Id = P.Patient_Id
    WHERE @Role = N'Admin'
       OR P.Contact_Number = @ContactNumber;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_GetAppointmentById
    @AppointmentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        A.Appointment_Id AS AppointmentId,
        A.Patient_Id AS PatientId,
        P.Full_Name AS PatientName,
        P.PatientEmail,
        A.Doctor_Id AS DoctorId,
        D.Full_Name AS DoctorName,
        A.AppointmentDate,
        A.Visit_Type AS VisitType,
        A.Diagnosis,
        A.Notes
    FROM dbo.Appointments AS A
    INNER JOIN dbo.Patients AS P ON A.Patient_Id = P.Patient_Id
    INNER JOIN dbo.Doctors AS D ON A.Doctor_Id = D.Doctor_Id
    WHERE A.Appointment_Id = @AppointmentId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_RegisterUser
    @FullName NVARCHAR(100),
    @Mobile NVARCHAR(20),
    @PasswordHash NVARCHAR(100),
    @Role NVARCHAR(30)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Users WHERE Mobile = @Mobile)
        RETURN 0;

    INSERT INTO dbo.Users (Full_Name, Mobile, PasswordHash, Role)
    VALUES (@FullName, @Mobile, @PasswordHash, COALESCE(NULLIF(@Role, N''), N'Patient'));
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_LoginUser
    @Mobile NVARCHAR(20),
    @PasswordHash NVARCHAR(100)
AS
BEGIN
    SELECT
        Full_Name AS FullName,
        Mobile,
        Role
    FROM dbo.Users
    WHERE Mobile = @Mobile
      AND PasswordHash = @PasswordHash;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Department)
BEGIN
    INSERT INTO dbo.Department (DepartmentName, Description)
    VALUES
        (N'Cardiology', N'Heart and cardiovascular care'),
        (N'Dermatology', N'Skin, hair, and nail care'),
        (N'Neurology', N'Brain and nervous system care'),
        (N'Pediatrics', N'Medical care for children'),
        (N'Orthopedics', N'Bone, joint, and muscle care'),
        (N'General Medicine', N'Primary and preventive care');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Mobile = N'01710000000')
BEGIN
    INSERT INTO dbo.Users (Full_Name, Mobile, PasswordHash, Role)
    VALUES
        (N'Test Patient', N'01710000000', N'oQnjaUetVt4dyhzEnw74rJrZp7GqDfQfs8TLc8H/Aeo=', N'Patient');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Patients)
BEGIN
    INSERT INTO dbo.Patients (Full_Name, DateOfBirth, Gender, Contact_Number, Address)
    VALUES
        (N'Aisha Rahman', '1992-04-18', N'Female', N'01710000001', N'Dhaka'),
        (N'Karim Hasan', '1986-09-02', N'Male', N'01710000002', N'Chattogram'),
        (N'Nusrat Jahan', '1998-01-25', N'Female', N'01710000003', N'Rajshahi'),
        (N'Tanvir Ahmed', '1979-11-11', N'Male', N'01710000004', N'Khulna'),
        (N'Sadia Islam', '2001-06-30', N'Female', N'01710000005', N'Sylhet'),
        (N'Imran Hossain', '1990-12-07', N'Male', N'01710000006', N'Barishal');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Doctors)
BEGIN
    INSERT INTO dbo.Doctors (Full_Name, Specialization, Contact_Number)
    SELECT Full_Name, Specialization, Contact_Number
    FROM (VALUES
        (N'Dr. Farhan Kabir', N'Cardiologist', N'01810000001'),
        (N'Dr. Mehedi Karim', N'Dermatologist', N'01810000002'),
        (N'Dr. Rima Sultana', N'Neurologist', N'01810000003'),
        (N'Dr. Anika Chowdhury', N'Pediatrician', N'01810000004'),
        (N'Dr. Omar Faisal', N'Orthopedic Surgeon', N'01810000005'),
        (N'Dr. Nusrat Nahar', N'General Physician', N'01810000006')
    ) AS Seed(Full_Name, Specialization, Contact_Number);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Medicines)
BEGIN
    INSERT INTO dbo.Medicines (Name, Description)
    VALUES
        (N'Paracetamol', N'Pain and fever relief'),
        (N'Amoxicillin', N'Antibiotic medicine'),
        (N'Cetirizine', N'Allergy symptom relief'),
        (N'Omeprazole', N'Reduces stomach acid'),
        (N'Ibuprofen', N'Pain and inflammation relief'),
        (N'Vitamin D3', N'Vitamin D supplement');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Appointments)
BEGIN
    INSERT INTO dbo.Appointments
        (Patient_Id, Doctor_Id, AppointmentDate, Visit_Type, Diagnosis, Notes)
    VALUES
        (1, 1, '2026-09-01 09:00:00', N'Consultation', N'Mild hypertension', N'Check blood pressure and review diet.'),
        (2, 2, '2026-09-02 10:30:00', N'Follow-up', N'Contact dermatitis', N'Continue skin care routine.'),
        (3, 3, '2026-09-03 11:00:00', N'Consultation', N'Recurrent headache', N'Keep a headache and sleep diary.'),
        (4, 4, '2026-09-04 14:00:00', N'Consultation', N' seasonal allergy', N'Bring previous test reports.'),
        (5, 5, '2026-09-05 15:30:00', N'Follow-up', N'Knee pain', N'Assess mobility after physiotherapy.'),
        (6, 6, '2026-09-06 16:00:00', N'Checkup', N'Annual health review', N'Complete routine blood tests.');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PrescriptionDetails)
BEGIN
    INSERT INTO dbo.PrescriptionDetails
        (Appointment_Id, Medicine_Id, Dosage, Start_Date, End_Date, Notes)
    VALUES
        (1, 1, N'500 mg twice daily', '2026-09-01', '2026-09-05', N'Take after meals.'),
        (2, 3, N'10 mg once daily', '2026-09-02', '2026-09-09', N'Take in the evening.'),
        (3, 1, N'500 mg as needed', '2026-09-03', '2026-09-10', N'Do not exceed four doses daily.'),
        (4, 3, N'10 mg once daily', '2026-09-04', '2026-09-11', N'Avoid known allergens.'),
        (5, 5, N'400 mg twice daily', '2026-09-05', '2026-09-10', N'Take with food.'),
        (6, 6, N'1000 IU once daily', '2026-09-06', '2026-10-06', N'Take with breakfast.');
END;
GO

SELECT N'Department' AS TableName, COUNT(*) AS RowCount FROM dbo.Department
UNION ALL SELECT N'Patients', COUNT(*) FROM dbo.Patients
UNION ALL SELECT N'Doctors', COUNT(*) FROM dbo.Doctors
UNION ALL SELECT N'Medicines', COUNT(*) FROM dbo.Medicines
UNION ALL SELECT N'Appointments', COUNT(*) FROM dbo.Appointments
UNION ALL SELECT N'PrescriptionDetails', COUNT(*) FROM dbo.PrescriptionDetails;