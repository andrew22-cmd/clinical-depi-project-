-- ============================================================
--  CLINIC SYSTEM - SEED DATA SCRIPT
--  Run this AFTER your migrations have been applied.
--  Safe to re-run: uses IF NOT EXISTS / SET IDENTITY_INSERT.
-- ============================================================

-- ─────────────────────────────────────────────
-- 1. SPECIALITIES
-- ─────────────────────────────────────────────
SET IDENTITY_INSERT Specialities ON;

INSERT INTO Specialities (SpecialityId, Name, ConsultationFee)
SELECT * FROM (VALUES
    (1, N'طب عام',          200),
    (2, N'باطنة',            300),
    (3, N'أطفال',            350),
    (4, N'عظام',             450),
    (5, N'جلدية',            400),
    (6, N'قلب وأوعية دموية', 600),
    (7, N'نساء وتوليد',      500),
    (8, N'عيون',             350)
) AS v(SpecialityId, Name, ConsultationFee)
WHERE NOT EXISTS (SELECT 1 FROM Specialities WHERE SpecialityId = v.SpecialityId);

SET IDENTITY_INSERT Specialities OFF;


-- ─────────────────────────────────────────────
-- 2. USERS  (Manager + Secretaries + Doctors + Patients)
--    Passwords stored as plain-text (as your system does)
-- ─────────────────────────────────────────────
SET IDENTITY_INSERT Users ON;

INSERT INTO Users (UserId, FirstName, LastName, Email, Phone, Password, Role, EmailConfirmed)
SELECT * FROM (VALUES
    -- Manager
    (1,  N'أحمد',     N'المدير',    'manager@clinic.com',      '01000000001', 'Admin@123',   'Manager',   1),

    -- Secretaries
    (2,  N'سارة',     N'حسن',       'sara.hassan@clinic.com',  '01000000002', 'Sara@123',    'Secretary', 1),
    (3,  N'مريم',     N'علي',       'mariam.ali@clinic.com',   '01000000003', 'Mariam@123',  'Secretary', 1),

    -- Doctors
    (4,  N'محمد',     N'خالد',      'dr.khalid@clinic.com',    '01000000004', 'Doctor@123',  'Doctor',    1),
    (5,  N'هاني',     N'إبراهيم',   'dr.hani@clinic.com',      '01000000005', 'Doctor@123',  'Doctor',    1),
    (6,  N'نورا',     N'سامي',      'dr.nora@clinic.com',      '01000000006', 'Doctor@123',  'Doctor',    1),
    (7,  N'كريم',     N'منصور',     'dr.karim@clinic.com',     '01000000007', 'Doctor@123',  'Doctor',    1),
    (8,  N'داليا',    N'فؤاد',      'dr.dalia@clinic.com',     '01000000008', 'Doctor@123',  'Doctor',    1),

    -- Patients (registered online)
    (9,  N'عمر',      N'شوقي',      'omar.shoky@gmail.com',    '01100000001', 'Omar@123',    'Patient',   1),
    (10, N'ليلى',     N'السيد',     'layla.said@gmail.com',    '01100000002', 'Layla@123',   'Patient',   1),
    (11, N'يوسف',     N'حامد',      'yousef.hamed@gmail.com',  '01100000003', 'Yousef@123',  'Patient',   1),
    (12, N'منى',      N'رضوان',     'mona.redwan@gmail.com',   '01100000004', 'Mona@123',    'Patient',   1),
    (13, N'تامر',     N'جاد',       'tamer.gad@gmail.com',     '01100000005', 'Tamer@123',   'Patient',   1)
) AS v(UserId, FirstName, LastName, Email, Phone, Password, Role, EmailConfirmed)
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE UserId = v.UserId);

SET IDENTITY_INSERT Users OFF;


-- ─────────────────────────────────────────────
-- 3. DOCTORS  (UserId → DoctorId, linked to Speciality)
-- ─────────────────────────────────────────────
SET IDENTITY_INSERT Doctors ON;

INSERT INTO Doctors (DoctorId, UserId, SpecialityId)
SELECT * FROM (VALUES
    (1, 4, 1),   -- د. محمد خالد    → طب عام
    (2, 5, 2),   -- د. هاني إبراهيم → باطنة
    (3, 6, 3),   -- د. نورا سامي    → أطفال
    (4, 7, 4),   -- د. كريم منصور   → عظام
    (5, 8, 6)    -- د. داليا فؤاد   → قلب وأوعية دموية
) AS v(DoctorId, UserId, SpecialityId)
WHERE NOT EXISTS (SELECT 1 FROM Doctors WHERE DoctorId = v.DoctorId);

SET IDENTITY_INSERT Doctors OFF;


-- ─────────────────────────────────────────────
-- 4. PATIENTS  (9→13 online, 14-16 offline walk-ins)
-- ─────────────────────────────────────────────
SET IDENTITY_INSERT Patients ON;

INSERT INTO Patients (PatientId, UserId, FullName, Phone, CreatedAt)
SELECT * FROM (VALUES
    (1, 9,    N'عمر شوقي',        '01100000001', '2025-01-10'),
    (2, 10,   N'ليلى السيد',      '01100000002', '2025-02-05'),
    (3, 11,   N'يوسف حامد',       '01100000003', '2025-03-15'),
    (4, 12,   N'منى رضوان',       '01100000004', '2025-04-20'),
    (5, 13,   N'تامر جاد',        '01100000005', '2025-05-01'),
    -- Offline (no UserId)
    (6, NULL, N'خالد عبد الله',   '01200000001', '2026-05-12'),
    (7, NULL, N'سمر إبراهيم',     '01200000002', '2026-06-03'),
    (8, NULL, N'وليد فاروق',      '01200000003', '2026-06-18')
) AS v(PatientId, UserId, FullName, Phone, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM Patients WHERE PatientId = v.PatientId);

SET IDENTITY_INSERT Patients OFF;


-- ─────────────────────────────────────────────
-- 5. DOCTOR SCHEDULES
--    Each doctor works on 2-3 days/week
--    StartTime / EndTime stored as TIME → use HH:mm:ss
-- ─────────────────────────────────────────────
SET IDENTITY_INSERT DoctorSchedules ON;

INSERT INTO DoctorSchedules (ScheduleId, DoctorId, WeekDay, StartTime, EndTime, IsActive)
SELECT * FROM (VALUES
    -- د. محمد خالد (DoctorId=1) - طب عام
    (1,  1, N'السبت',    '09:00:00', '13:00:00', 1),
    (2,  1, N'الإثنين',  '09:00:00', '13:00:00', 1),
    (3,  1, N'الأربعاء', '09:00:00', '12:00:00', 1),

    -- د. هاني إبراهيم (DoctorId=2) - باطنة
    (4,  2, N'الأحد',    '10:00:00', '14:00:00', 1),
    (5,  2, N'الثلاثاء', '10:00:00', '14:00:00', 1),

    -- د. نورا سامي (DoctorId=3) - أطفال
    (6,  3, N'السبت',    '14:00:00', '18:00:00', 1),
    (7,  3, N'الإثنين',  '14:00:00', '18:00:00', 1),
    (8,  3, N'الأربعاء', '14:00:00', '17:00:00', 1),

    -- د. كريم منصور (DoctorId=4) - عظام
    (9,  4, N'الأحد',    '09:00:00', '12:00:00', 1),
    (10, 4, N'الخميس',   '09:00:00', '12:00:00', 1),

    -- د. داليا فؤاد (DoctorId=5) - قلب
    (11, 5, N'الثلاثاء', '16:00:00', '20:00:00', 1),
    (12, 5, N'الخميس',   '16:00:00', '20:00:00', 1)
) AS v(ScheduleId, DoctorId, WeekDay, StartTime, EndTime, IsActive)
WHERE NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE ScheduleId = v.ScheduleId);

SET IDENTITY_INSERT DoctorSchedules OFF;


-- ─────────────────────────────────────────────
-- 6. DOCTOR SCHEDULE SLOTS  (كل 30 دقيقة)
--    ScheduleId, SlotTime, IsBooked, CreatedAt
-- ─────────────────────────────────────────────
SET IDENTITY_INSERT DoctorScheduleSlots ON;

INSERT INTO DoctorScheduleSlots (SlotId, ScheduleId, SlotTime, IsBooked, CreatedAt)
SELECT * FROM (VALUES
    -- Schedule 1: د. محمد - السبت 09:00-13:00 (8 slots)
    (1,  1, '09:00:00', 1, '2026-01-01'),
    (2,  1, '09:30:00', 1, '2026-01-01'),
    (3,  1, '10:00:00', 1, '2026-01-01'),
    (4,  1, '10:30:00', 0, '2026-01-01'),
    (5,  1, '11:00:00', 0, '2026-01-01'),
    (6,  1, '11:30:00', 0, '2026-01-01'),
    (7,  1, '12:00:00', 0, '2026-01-01'),
    (8,  1, '12:30:00', 0, '2026-01-01'),

    -- Schedule 2: د. محمد - الإثنين 09:00-13:00
    (9,  2, '09:00:00', 1, '2026-01-01'),
    (10, 2, '09:30:00', 0, '2026-01-01'),
    (11, 2, '10:00:00', 0, '2026-01-01'),
    (12, 2, '10:30:00', 0, '2026-01-01'),
    (13, 2, '11:00:00', 0, '2026-01-01'),
    (14, 2, '11:30:00', 0, '2026-01-01'),
    (15, 2, '12:00:00', 0, '2026-01-01'),
    (16, 2, '12:30:00', 0, '2026-01-01'),

    -- Schedule 3: د. محمد - الأربعاء 09:00-12:00
    (17, 3, '09:00:00', 0, '2026-01-01'),
    (18, 3, '09:30:00', 0, '2026-01-01'),
    (19, 3, '10:00:00', 1, '2026-01-01'),
    (20, 3, '10:30:00', 0, '2026-01-01'),
    (21, 3, '11:00:00', 0, '2026-01-01'),
    (22, 3, '11:30:00', 0, '2026-01-01'),

    -- Schedule 4: د. هاني - الأحد 10:00-14:00
    (23, 4, '10:00:00', 1, '2026-01-01'),
    (24, 4, '10:30:00', 1, '2026-01-01'),
    (25, 4, '11:00:00', 0, '2026-01-01'),
    (26, 4, '11:30:00', 0, '2026-01-01'),
    (27, 4, '12:00:00', 0, '2026-01-01'),
    (28, 4, '12:30:00', 0, '2026-01-01'),
    (29, 4, '13:00:00', 0, '2026-01-01'),
    (30, 4, '13:30:00', 0, '2026-01-01'),

    -- Schedule 5: د. هاني - الثلاثاء 10:00-14:00
    (31, 5, '10:00:00', 1, '2026-01-01'),
    (32, 5, '10:30:00', 0, '2026-01-01'),
    (33, 5, '11:00:00', 0, '2026-01-01'),
    (34, 5, '11:30:00', 0, '2026-01-01'),
    (35, 5, '12:00:00', 0, '2026-01-01'),
    (36, 5, '12:30:00', 0, '2026-01-01'),
    (37, 5, '13:00:00', 0, '2026-01-01'),
    (38, 5, '13:30:00', 0, '2026-01-01'),

    -- Schedule 6: د. نورا - السبت 14:00-18:00
    (39, 6, '14:00:00', 1, '2026-01-01'),
    (40, 6, '14:30:00', 1, '2026-01-01'),
    (41, 6, '15:00:00', 0, '2026-01-01'),
    (42, 6, '15:30:00', 0, '2026-01-01'),
    (43, 6, '16:00:00', 0, '2026-01-01'),
    (44, 6, '16:30:00', 0, '2026-01-01'),
    (45, 6, '17:00:00', 0, '2026-01-01'),
    (46, 6, '17:30:00', 0, '2026-01-01'),

    -- Schedule 7: د. نورا - الإثنين 14:00-18:00
    (47, 7, '14:00:00', 0, '2026-01-01'),
    (48, 7, '14:30:00', 0, '2026-01-01'),
    (49, 7, '15:00:00', 1, '2026-01-01'),
    (50, 7, '15:30:00', 0, '2026-01-01'),
    (51, 7, '16:00:00', 0, '2026-01-01'),
    (52, 7, '16:30:00', 0, '2026-01-01'),
    (53, 7, '17:00:00', 0, '2026-01-01'),
    (54, 7, '17:30:00', 0, '2026-01-01'),

    -- Schedule 9: د. كريم - الأحد 09:00-12:00
    (55, 9,  '09:00:00', 1, '2026-01-01'),
    (56, 9,  '09:30:00', 0, '2026-01-01'),
    (57, 9,  '10:00:00', 0, '2026-01-01'),
    (58, 9,  '10:30:00', 0, '2026-01-01'),
    (59, 9,  '11:00:00', 0, '2026-01-01'),
    (60, 9,  '11:30:00', 0, '2026-01-01'),

    -- Schedule 11: د. داليا - الثلاثاء 16:00-20:00
    (61, 11, '16:00:00', 1, '2026-01-01'),
    (62, 11, '16:30:00', 1, '2026-01-01'),
    (63, 11, '17:00:00', 0, '2026-01-01'),
    (64, 11, '17:30:00', 0, '2026-01-01'),
    (65, 11, '18:00:00', 0, '2026-01-01'),
    (66, 11, '18:30:00', 0, '2026-01-01'),
    (67, 11, '19:00:00', 0, '2026-01-01'),
    (68, 11, '19:30:00', 0, '2026-01-01')
) AS v(SlotId, ScheduleId, SlotTime, IsBooked, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM DoctorScheduleSlots WHERE SlotId = v.SlotId);

SET IDENTITY_INSERT DoctorScheduleSlots OFF;


-- ─────────────────────────────────────────────
-- 7. RESERVATIONS
--    منطقية: كل حجز مرتبط بـ PatientId / DoctorId / SlotId
--    التواريخ قريبة من اليوم (10 يوليو 2026)
-- ─────────────────────────────────────────────
SET IDENTITY_INSERT Reservations ON;

INSERT INTO Reservations (ReservationId, PatientId, DoctorId, SlotId, ReservationDate, Status, Source, CreatedBy, CreatedAt)
SELECT * FROM (VALUES
    -- ✅ Completed (عيانين اتكشفوا فعلاً)
    (1, 1, 1, 1,  '2026-06-21', 'Completed', 'Online',  9,  '2026-06-19'),  -- عمر → د.محمد (السبت)
    (2, 2, 2, 23, '2026-06-22', 'Completed', 'Online',  10, '2026-06-20'),  -- ليلى → د.هاني (الأحد)
    (3, 6, 3, 39, '2026-06-21', 'Completed', 'Offline', 2,  '2026-06-21'),  -- خالد → د.نورا (السبت offline)
    (4, 3, 1, 2,  '2026-06-21', 'Completed', 'Online',  11, '2026-06-19'),  -- يوسف → د.محمد (السبت)
    (5, 4, 5, 61, '2026-07-01', 'Completed', 'Online',  12, '2026-06-30'),  -- منى → د.داليا (الثلاثاء)
    (6, 7, 4, 55, '2026-06-29', 'Completed', 'Offline', 2,  '2026-06-29'),  -- سمر → د.كريم (الأحد)

    -- ✅ Confirmed (حجوزات قادمة مؤكدة)
    (7,  5, 2, 24, '2026-07-13', 'Confirmed', 'Online',  13, '2026-07-09'),  -- تامر → د.هاني (الأحد)
    (8,  1, 3, 40, '2026-07-12', 'Confirmed', 'Online',  9,  '2026-07-10'),  -- عمر → د.نورا (السبت)
    (9,  8, 1, 3,  '2026-07-12', 'Confirmed', 'Offline', 3,  '2026-07-10'),  -- وليد → د.محمد (السبت)
    (10, 2, 5, 62, '2026-07-15', 'Confirmed', 'Online',  10, '2026-07-10'),  -- ليلى → د.داليا (الثلاثاء)
    (11, 3, 2, 31, '2026-07-15', 'Confirmed', 'Online',  11, '2026-07-10'),  -- يوسف → د.هاني (الثلاثاء)

    -- ❌ Cancelled
    (12, 4, 1, 9,  '2026-07-07', 'Cancelled', 'Online',  12, '2026-07-05'),  -- منى → د.محمد (الإثنين)
    (13, 6, 3, 49, '2026-07-07', 'Cancelled', 'Offline', 2,  '2026-07-06')   -- خالد → د.نورا (الإثنين)
) AS v(ReservationId, PatientId, DoctorId, SlotId, ReservationDate, Status, Source, CreatedBy, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM Reservations WHERE ReservationId = v.ReservationId);

SET IDENTITY_INSERT Reservations OFF;


-- ─────────────────────────────────────────────
-- 8. PATIENT NOTES  (للحجوزات المكتملة فقط)
-- ─────────────────────────────────────────────
SET IDENTITY_INSERT PatientNotes ON;

INSERT INTO PatientNotes (NoteId, ReservationId, PatientId, DoctorId, Notes, Diagnosis, VisitDate, CreatedAt)
SELECT * FROM (VALUES
    (1, 1, 1, 1,
        N'المريض يشكو من ارتفاع درجة الحرارة والتعب العام منذ 3 أيام. لا توجد أعراض تنفسية.',
        N'التهاب فيروسي حاد',
        '2026-06-21', '2026-06-21'),

    (2, 2, 2, 2,
        N'ارتفاع ضغط الدم - القراءة 160/100. يعاني من صداع متكرر.',
        N'ارتفاع ضغط الدم المرحلة الثانية',
        '2026-06-22', '2026-06-22'),

    (3, 3, 6, 3,
        N'طفل عمره 4 سنوات، يعاني من ألم في الأذن وحرارة 38.5. الأذن محتقنة.',
        N'التهاب الأذن الوسطى الحاد',
        '2026-06-21', '2026-06-21'),

    (4, 4, 3, 1,
        N'مراجعة روتينية. المريض يشعر بتحسن. تم إيقاف مضاد الحيوي.',
        N'التهاب اللوزتين - في طريق الشفاء',
        '2026-06-21', '2026-06-21'),

    (5, 5, 4, 5,
        N'تخطيط القلب طبيعي. الضغط 120/80. يشكو من خفقان متقطع.',
        N'خفقان القلب الوظيفي - لا يستدعي علاجاً',
        '2026-07-01', '2026-07-01'),

    (6, 6, 7, 4,
        N'ألم في الركبة اليسرى بعد حادثة رياضية. الأشعة تظهر التواء في الرباط.',
        N'التواء الرباط الجانبي للركبة اليسرى - درجة أولى',
        '2026-06-29', '2026-06-29')
) AS v(NoteId, ReservationId, PatientId, DoctorId, Notes, Diagnosis, VisitDate, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM PatientNotes WHERE NoteId = v.NoteId);

SET IDENTITY_INSERT PatientNotes OFF;

-- ─────────────────────────────────────────────
-- DONE ✅
-- ─────────────────────────────────────────────
PRINT 'Seed data inserted successfully!';
