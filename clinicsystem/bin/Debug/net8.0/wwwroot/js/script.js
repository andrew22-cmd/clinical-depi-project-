function readProfileFromDOM() {
    const el = document.getElementById('app-user-profile');
    return {
        name: el?.dataset.name || 'مستخدم جديد',
        phone: el?.dataset.phone || '',
        email: el?.dataset.email || ''
    };
}

function readRoleFromDOM() {
    const el = document.getElementById('app-user-profile');
    return el?.dataset.role || '';
}


// ── Navigation ────────────────────────────────────────────────────────────────
function goTo(page) {
    window.location.href = page;
}

function goBackFromDoctorsSchedules() {
    const role = readRoleFromDOM();
    if (role === 'manager') { goTo('/Manager/Dashboard'); return; }
    if (role === 'secretary') { goTo('/Secretary/Dashboard'); return; }
    goTo('/Home/Index');
}


// ── Logout ────────────────────────────────────────────────────────────────────
// Delegates entirely to the server — Identity cookie is invalidated by
// AccountController.Logout which calls SignOutAsync().
function logout() {
    localStorage.removeItem('doctorSchedules');
    localStorage.removeItem('reservations');
    localStorage.removeItem('lastReservation');
    localStorage.removeItem('currentDoctorId');
    window.location.href = '/Account/Logout';
}


// ── Static doctor fallback data ───────────────────────────────────────────────
// Used until ScheduleController serves this from the database.
const doctorsData = [
    {
        id: 'john', name: 'د. جون إبراهيم', specialty: 'باطنة',
        days: [
            {
                day: 'الثلاثاء', from: '12:00 م', to: '04:00 م',
                slots: ['12:00 م', '12:30 م', '01:00 م', '01:30 م', '02:00 م', '03:00 م']
            },
            {
                day: 'الخميس', from: '10:00 ص', to: '12:00 م',
                slots: ['10:00 ص', '10:30 ص', '11:00 ص', '11:30 ص']
            }
        ]
    },
    {
        id: 'sara', name: 'د. سارة جونسون', specialty: 'قلب',
        days: [
            {
                day: 'الأحد', from: '09:00 ص', to: '01:00 م',
                slots: ['09:00 ص', '10:00 ص', '11:00 ص', '12:00 م']
            },
            {
                day: 'الإثنين', from: '09:00 ص', to: '02:00 م',
                slots: ['09:00 ص', '10:00 ص', '11:00 ص', '12:00 م', '01:00 م']
            }
        ]
    },
    {
        id: 'mohamed', name: 'د. محمد علي', specialty: 'مخ وأعصاب',
        days: [
            {
                day: 'الإثنين', from: '10:00 ص', to: '04:00 م',
                slots: ['10:00 ص', '11:00 ص', '12:00 م', '01:00 م', '02:00 م', '03:00 م']
            },
            {
                day: 'الأربعاء', from: '11:00 ص', to: '03:00 م',
                slots: ['11:00 ص', '12:00 م', '01:00 م', '02:00 م']
            }
        ]
    },
    {
        id: 'rana', name: 'د. رنا سعد', specialty: 'أطفال',
        days: [
            {
                day: 'السبت', from: '02:00 م', to: '06:00 م',
                slots: ['02:00 م', '03:00 م', '04:00 م', '05:00 م']
            },
            {
                day: 'الثلاثاء', from: '10:00 ص', to: '01:00 م',
                slots: ['10:00 ص', '11:00 ص', '12:00 م']
            }
        ]
    }
];

function getDoctorsData() {
    const raw = localStorage.getItem('doctorSchedules');
    if (!raw) return JSON.parse(JSON.stringify(doctorsData));
    try {
        const parsed = JSON.parse(raw);
        if (Array.isArray(parsed) && parsed.length > 0) return parsed;
    } catch { /* ignore */ }
    return JSON.parse(JSON.stringify(doctorsData));
}

function saveDoctorsData(data) {
    localStorage.setItem('doctorSchedules', JSON.stringify(data));
}


// ── Reservations (pending backend migration) ──────────────────────────────────
function getReservations() {
    const data = localStorage.getItem('reservations');
    if (!data) {
        const profile = readProfileFromDOM();
        return [{
            patient: profile.name, phone: profile.phone,
            doctor: 'د. سارة جونسون', day: 'الإثنين',
            time: '10:00 ص', source: 'online',
            dateISO: new Date().toISOString().slice(0, 10)
        }];
    }
    try {
        const list = JSON.parse(data);
        if (!Array.isArray(list)) return [];
        return list.map((r) => ({ ...r, dateISO: r.dateISO || new Date().toISOString().slice(0, 10) }));
    } catch { return []; }
}

function saveReservation(item) {
    const list = getReservations();
    list.unshift({ ...item, dateISO: item.dateISO || new Date().toISOString().slice(0, 10) });
    localStorage.setItem('reservations', JSON.stringify(list));
}


// ── Date utility ──────────────────────────────────────────────────────────────
function getTodayArabicName() {
    return ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'][new Date().getDay()];
}


// ── Rendering: patient reservation history ────────────────────────────────────
function renderReservationHistory() {
    const target = document.getElementById('userReservationHistory');
    if (!target) return;
    target.innerHTML = getReservations().slice(0, 5).map((r) =>
        `<tr><td>${r.doctor}</td><td>${r.day}</td><td>${r.time}</td>
         <td><span class="badge">${r.source === 'online' ? 'أونلاين' : 'سكرتير'}</span></td></tr>`
    ).join('');
}


// ── Rendering: secretary booking list ────────────────────────────────────────
function fillSecretaryBookings() {
    const target = document.getElementById('bookingListBody');
    if (!target) return;
    const list = getReservations();
    target.innerHTML = list.slice(0, 8).map((r) =>
        `<tr><td>${r.patient}</td><td>${r.phone}</td><td>${r.doctor}</td>
         <td>${r.day}</td><td>${r.time}</td>
         <td><span class="badge">${r.source === 'online' ? 'حجز أونلاين' : 'حجز يدوي'}</span></td></tr>`
    ).join('');
    const count = document.getElementById('reservationCounter');
    if (count) count.textContent = String(list.length);
}


// ── Rendering: doctor cards ───────────────────────────────────────────────────
function renderDoctorsList(targetId) {
    const target = document.getElementById(targetId);
    if (!target) return;
    target.innerHTML = getDoctorsData().map((d) => {
        const days = d.days.map((x) => `<li>${x.day}: ${x.from} - ${x.to}</li>`).join('');
        return `<div class="card"><h4>${d.name}</h4><p class="muted">${d.specialty}</p>
                <ul class="list">${days}</ul></div>`;
    }).join('');
}


// ── Rendering: today's schedules ──────────────────────────────────────────────
function renderTodayDoctorSchedules() {
    const target = document.getElementById('todayDoctorSchedulesBody');
    if (!target) return;
    const today = getTodayArabicName();
    const rows = [];
    getDoctorsData().forEach((doctor) => {
        const shifts = doctor.days.filter((d) => d.day === today);
        if (!shifts.length) {
            rows.push(`<tr><td>${doctor.name}</td><td>${doctor.specialty}</td>
                       <td>${today}</td><td>-</td><td>-</td><td>غير متاح اليوم</td></tr>`);
            return;
        }
        shifts.forEach((s) =>
            rows.push(`<tr><td>${doctor.name}</td><td>${doctor.specialty}</td>
                       <td>${s.day}</td><td>${s.from}</td><td>${s.to}</td>
                       <td><span class="badge">متاح</span></td></tr>`)
        );
    });
    target.innerHTML = rows.join('');
}


// ── Schedule/Index: manager-only actions ──────────────────────────────────────
// Role comes from #app-user-profile (server-rendered) — NEVER localStorage.
function toggleManagerScheduleActions() {
    const el = document.getElementById('managerScheduleActions');
    if (!el) return;
    el.style.display = readRoleFromDOM() === 'manager' ? 'flex' : 'none';
}


// ── Manager dashboard stats ───────────────────────────────────────────────────
function renderManagerDashboardStats() {
    const totalUsersEl = document.getElementById('statTotalUsers');
    const totalDoctorsEl = document.getElementById('statTotalDoctors');
    const totalSecretariesEl = document.getElementById('statTotalSecretaries');
    if (!totalUsersEl || !totalDoctorsEl || !totalSecretariesEl) return;

    const reservations = getReservations();
    const allDoctors = getDoctorsData();
    const today = getTodayArabicName();
    const uniquePatients = new Set(reservations.map((r) => `${r.patient}|${r.phone}`)).size;
    const doctorsToday = allDoctors.filter((d) => d.days.some((x) => x.day === today)).length;
    const availableSlots = allDoctors.reduce((sum, d) =>
        sum + d.days.filter((x) => x.day === today)
            .reduce((s, x) => s + (Array.isArray(x.slots) ? x.slots.length : 0), 0), 0);
    const bookedToday = reservations.filter((r) => r.day === today).length;
    const occupancy = availableSlots > 0
        ? Math.min(100, Math.round((bookedToday / availableSlots) * 100)) : 0;

    totalUsersEl.textContent = String(60 + uniquePatients);
    totalDoctorsEl.textContent = String(allDoctors.length);
    // Secretary count will come from backend — hardcoded fallback for now
    totalSecretariesEl.textContent = '4';

    const weeklyEl = document.getElementById('reportWeeklyReservations');
    const occupancyEl = document.getElementById('reportOccupancy');
    const todayEl = document.getElementById('reportDoctorsToday');
    if (weeklyEl) weeklyEl.textContent = String(reservations.length);
    if (occupancyEl) occupancyEl.textContent = `${occupancy}%`;
    if (todayEl) todayEl.textContent = String(doctorsToday);
}


// ── Doctor dashboard ──────────────────────────────────────────────────────────
function renderDoctorDashboard() {
    const nameEl = document.getElementById('doctorNameHeader');
    const summaryEl = document.getElementById('doctorSummaryHeader');
    const totalEl = document.getElementById('doctorTodayAppointments');
    const confirmedEl = document.getElementById('doctorConfirmedAppointments');
    const availEl = document.getElementById('doctorAvailableTime');
    const calendarEl = document.getElementById('doctorCalendarGrid');
    const appointmentsBody = document.getElementById('doctorAppointmentsBody');
    if (!nameEl || !summaryEl || !totalEl || !confirmedEl
        || !availEl || !calendarEl || !appointmentsBody) return;

    // Use server-rendered name from DOM, fall back to first doctor in list
    const serverName = readProfileFromDOM().name;
    const allDoctors = getDoctorsData();
    const doctor = allDoctors.find((d) =>
        serverName && d.name.includes(serverName.split(' ').pop())
    ) || allDoctors[0] || null;

    if (!doctor) {
        nameEl.textContent = 'لا يوجد طبيب';
        summaryEl.textContent = 'لا توجد بيانات متاحة.';
        appointmentsBody.innerHTML = '<tr><td colspan="4" class="muted">لا توجد بيانات.</td></tr>';
        return;
    }

    nameEl.textContent = doctor.name;
    summaryEl.textContent = `الأيام: ${doctor.days.map((d) => d.day).join(' - ')} | التخصص: ${doctor.specialty}`;
    calendarEl.innerHTML = doctor.days.map((d) =>
        `<div class="day-card"><strong>${d.day}</strong>
         <p class="muted">${d.from} - ${d.to}</p></div>`
    ).join('');

    const reservations = getReservations().filter((r) => (r.doctor || '').trim() === doctor.name);
    const today = getTodayArabicName();
    const todayReservations = reservations.filter((r) => r.day === today);
    const slotsToday = doctor.days
        .filter((d) => d.day === today)
        .reduce((sum, d) => sum + (Array.isArray(d.slots) ? d.slots.length : 0), 0);

    totalEl.textContent = String(todayReservations.length);
    confirmedEl.textContent = String(todayReservations.length);
    availEl.textContent = `${Math.max(0, slotsToday - todayReservations.length) * 0.5}h`;

    appointmentsBody.innerHTML = reservations.length === 0
        ? '<tr><td colspan="4" class="muted">لا توجد حجوزات لهذا الطبيب حتى الآن.</td></tr>'
        : reservations.slice(0, 8).map((r) =>
            `<tr><td>${r.patient || '-'}</td><td>${r.day || '-'}</td>
             <td>${r.time || '-'}</td><td><span class="badge">مؤكد</span></td></tr>`
        ).join('');
}


// ── Reservation booking page ──────────────────────────────────────────────────
function initReservationPage() {
    const specialtySelect = document.getElementById('specialtySelect');
    const doctorSelect = document.getElementById('doctorSelect');
    const availability = document.getElementById('doctorAvailability');
    const bookBtn = document.getElementById('bookBtn');
    const reservationMsg = document.getElementById('reservationMsg');
    if (!specialtySelect || !doctorSelect || !availability || !bookBtn) return;

    let selected = null;
    const allDoctors = getDoctorsData();
    const specialties = [...new Set(allDoctors.map((d) => d.specialty))];

    specialtySelect.innerHTML = '<option value="">اختر التخصص</option>' +
        specialties.map((s) => `<option value="${s}">${s}</option>`).join('');

    function fillDoctors() {
        const spec = specialtySelect.value;
        const filtered = spec ? allDoctors.filter((d) => d.specialty === spec) : allDoctors;
        doctorSelect.innerHTML = '<option value="">اختر الطبيب</option>' +
            filtered.map((d) => `<option value="${d.id}">${d.name}</option>`).join('');
        availability.innerHTML = '<p class="muted">اختر طبيباً لعرض أيامه وساعاته المتاحة.</p>';
        selected = null;
        bookBtn.disabled = true;
    }

    function renderAvailability() {
        const doctor = allDoctors.find((d) => d.id === doctorSelect.value);
        if (!doctor) {
            availability.innerHTML = '<p class="muted">اختر طبيباً لعرض أيامه وساعاته المتاحة.</p>';
            return;
        }
        availability.innerHTML = doctor.days.map((day, di) => {
            const slots = day.slots.map((slot, i) =>
                `<button type="button" class="slot-btn"
                    data-day="${day.day}" data-time="${slot}"
                    data-doctor="${doctor.name}" data-idx="${di}-${i}">${slot}</button>`
            ).join('');
            return `<div class="schedule-box"><strong>${day.day}</strong>
                    <span class="muted">(${day.from} - ${day.to})</span>
                    <div class="slot-grid">${slots}</div></div>`;
        }).join('');

        availability.querySelectorAll('.slot-btn').forEach((btn) => {
            btn.addEventListener('click', () => {
                availability.querySelectorAll('.slot-btn')
                    .forEach((x) => x.classList.remove('active'));
                btn.classList.add('active');
                selected = { doctor: btn.dataset.doctor, day: btn.dataset.day, time: btn.dataset.time };
                if (reservationMsg)
                    reservationMsg.textContent =
                        `تم اختيار: ${selected.doctor} - ${selected.day} - ${selected.time}`;
                bookBtn.disabled = false;
            });
        });
    }

    specialtySelect.addEventListener('change', fillDoctors);
    doctorSelect.addEventListener('change', renderAvailability);
    fillDoctors();

    bookBtn.addEventListener('click', () => {
        if (!selected) return;
        // Profile from server DOM — NEVER localStorage
        const profile = readProfileFromDOM();
        const item = { patient: profile.name, phone: profile.phone, ...selected, source: 'online' };
        saveReservation(item);
        localStorage.setItem('lastReservation', JSON.stringify(item));
        goTo('/Reservation/Info');
    });
}


// ── Reservation summary ───────────────────────────────────────────────────────
function fillReservationSummary() {
    const data = localStorage.getItem('lastReservation');
    if (!data) return;
    let item;
    try { item = JSON.parse(data); } catch { return; }
    const doctorEl = document.getElementById('resDoctor');
    const dayEl = document.getElementById('resDay');
    const timeEl = document.getElementById('resTime');
    if (doctorEl) doctorEl.textContent = item.doctor;
    if (dayEl) dayEl.textContent = item.day;
    if (timeEl) timeEl.textContent = item.time;
}


// ── Secretary: manual reservation ────────────────────────────────────────────
function createSecretaryReservation(event) {
    event.preventDefault();
    const patient = document.getElementById('secPatientName')?.value.trim();
    const phone = document.getElementById('secPatientPhone')?.value.trim();
    const doctor = document.getElementById('secDoctor')?.value;
    const day = document.getElementById('secDay')?.value;
    const time = document.getElementById('secTime')?.value;
    const msg = document.getElementById('secMsg');

    if (!patient || !phone || !doctor || !day || !time) {
        if (msg) msg.textContent = 'أكمل بيانات الحجز أولاً.';
        return;
    }
    saveReservation({ patient, phone, doctor, day, time, source: 'manual' });
    if (msg) msg.textContent = 'تمت إضافة الحجز بنجاح.';
    fillSecretaryBookings();
}


// ── Manager: reservations by date ────────────────────────────────────────────
function setReservationFilterToday() {
    const input = document.getElementById('reservationDateFilter');
    if (!input) return;
    input.value = new Date().toISOString().slice(0, 10);
    renderReservationsByDate();
}

function shiftReservationDate(deltaDays) {
    const input = document.getElementById('reservationDateFilter');
    if (!input) return;
    const base = input.value ? new Date(input.value) : new Date();
    base.setDate(base.getDate() + deltaDays);
    input.value = base.toISOString().slice(0, 10);
    renderReservationsByDate();
}

function renderReservationsByDate() {
    const body = document.getElementById('reservationListByDateBody');
    const input = document.getElementById('reservationDateFilter');
    if (!body || !input) return;
    if (!input.value) input.value = new Date().toISOString().slice(0, 10);
    const reservations = getReservations().filter((r) => r.dateISO === input.value);
    body.innerHTML = reservations.length === 0
        ? '<tr><td colspan="6" class="muted">لا توجد حجوزات في هذا التاريخ.</td></tr>'
        : reservations.map((r) =>
            `<tr><td>${r.patient || '-'}</td><td>${r.phone || '-'}</td>
             <td>${r.doctor || '-'}</td><td>${r.day || '-'}</td><td>${r.time || '-'}</td>
             <td><span class="badge">${r.source === 'online' ? 'أونلاين' : 'يدوي'}</span></td></tr>`
        ).join('');
}


// ── Schedule CRUD ─────────────────────────────────────────────────────────────
function normalizeSlots(slotsText) {
    if (!slotsText) return [];
    return slotsText.split(',').map((x) => x.trim()).filter(Boolean);
}

function addDoctorSchedule(event) {
    event.preventDefault();
    const name = document.getElementById('newDoctorName')?.value.trim();
    const specialty = document.getElementById('newDoctorSpecialty')?.value.trim();
    const day = document.getElementById('newDoctorDay')?.value;
    const from = document.getElementById('newDoctorFrom')?.value.trim();
    const to = document.getElementById('newDoctorTo')?.value.trim();
    const slots = normalizeSlots(document.getElementById('newDoctorSlots')?.value || '');
    const msg = document.getElementById('addScheduleMsg');
    if (!name || !specialty || !day || !from || !to) {
        if (msg) msg.textContent = 'من فضلك أكمل كل البيانات المطلوبة.';
        return;
    }
    const list = getDoctorsData();
    list.push({ id: `doc_${Date.now()}`, name, specialty, days: [{ day, from, to, slots }] });
    saveDoctorsData(list);
    if (msg) msg.textContent = 'تم إضافة الطبيب وجدوله بنجاح.';
    setTimeout(() => goTo('/Schedule/Index'), 700);
}

function initEditSchedulePage() {
    const select = document.getElementById('editDoctorSelect');
    if (!select) return;
    select.innerHTML = '<option value="">اختر الطبيب</option>' +
        getDoctorsData().map((d) =>
            `<option value="${d.id}">${d.name} - ${d.specialty}</option>`
        ).join('');
}

function updateDoctorSchedule(event) {
    event.preventDefault();
    const doctorId = document.getElementById('editDoctorSelect')?.value;
    const day = document.getElementById('editDoctorDay')?.value;
    const from = document.getElementById('editDoctorFrom')?.value.trim();
    const to = document.getElementById('editDoctorTo')?.value.trim();
    const slots = normalizeSlots(document.getElementById('editDoctorSlots')?.value || '');
    const msg = document.getElementById('editScheduleMsg');
    if (!doctorId || !day || !from || !to) {
        if (msg) msg.textContent = 'من فضلك اختر الطبيب وأدخل بيانات اليوم.';
        return;
    }
    const list = getDoctorsData();
    const doctor = list.find((d) => d.id === doctorId);
    if (!doctor) { if (msg) msg.textContent = 'الطبيب غير موجود.'; return; }
    const existing = doctor.days.find((d) => d.day === day);
    if (existing) { existing.from = from; existing.to = to; existing.slots = slots; }
    else { doctor.days.push({ day, from, to, slots }); }
    saveDoctorsData(list);
    if (msg) msg.textContent = 'تم تحديث جدول الطبيب بنجاح.';
    setTimeout(() => goTo('/Schedule/Index'), 700);
}

function deleteDoctorDaySchedule() {
    const doctorId = document.getElementById('editDoctorSelect')?.value;
    const day = document.getElementById('editDoctorDay')?.value;
    const msg = document.getElementById('editScheduleMsg');
    if (!doctorId || !day) { if (msg) msg.textContent = 'اختر الطبيب واليوم أولاً.'; return; }
    const list = getDoctorsData();
    const index = list.findIndex((d) => d.id === doctorId);
    if (index === -1) { if (msg) msg.textContent = 'الطبيب غير موجود.'; return; }
    const before = list[index].days.length;
    list[index].days = list[index].days.filter((d) => d.day !== day);
    if (list[index].days.length === before) {
        if (msg) msg.textContent = 'لا يوجد ميعاد لهذا اليوم ليتم حذفه.'; return;
    }
    if (list[index].days.length === 0) list.splice(index, 1);
    saveDoctorsData(list);
    if (msg) msg.textContent = 'تم حذف الميعاد بنجاح.';
    setTimeout(() => goTo('/Schedule/Index'), 700);
}

function deleteDoctorCompletely() {
    const doctorId = document.getElementById('editDoctorSelect')?.value;
    const msg = document.getElementById('editScheduleMsg');
    if (!doctorId) { if (msg) msg.textContent = 'اختر الطبيب أولاً.'; return; }
    const list = getDoctorsData();
    const doctor = list.find((d) => d.id === doctorId);
    if (!doctor) { if (msg) msg.textContent = 'الطبيب غير موجود.'; return; }
    if (!window.confirm(`هل تريد حذف الطبيب ${doctor.name} بالكامل؟`)) return;
    saveDoctorsData(list.filter((d) => d.id !== doctorId));
    if (msg) msg.textContent = 'تم حذف الطبيب بالكامل.';
    setTimeout(() => goTo('/Schedule/Index'), 700);
}


// ── Bootstrap ─────────────────────────────────────────────────────────────────
window.addEventListener('DOMContentLoaded', () => {
    renderReservationHistory();
    fillSecretaryBookings();
    renderDoctorsList('allDoctorsSchedules');
    renderDoctorsList('allDoctorsSchedulesPage');
    renderTodayDoctorSchedules();
    toggleManagerScheduleActions();
    renderManagerDashboardStats();
    renderDoctorDashboard();
    initReservationPage();
    initEditSchedulePage();
    fillReservationSummary();
    setReservationFilterToday();
    renderReservationsByDate();

    if (document.getElementById('statTotalUsers')) {
        setInterval(renderManagerDashboardStats, 10_000);
    }
});