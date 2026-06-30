const doctorsData = [
    {
        id: 'john',
        name: 'د. جون إبراهيم',
        specialty: 'باطنة',
        days: [
            { day: 'الثلاثاء', from: '12:00 م', to: '04:00 م', slots: ['12:00 م', '12:30 م', '01:00 م', '01:30 م', '02:00 م', '03:00 م'] },
            { day: 'الخميس', from: '10:00 ص', to: '12:00 م', slots: ['10:00 ص', '10:30 ص', '11:00 ص', '11:30 ص'] }
        ]
    },
    {
        id: 'sara',
        name: 'د. سارة جونسون',
        specialty: 'قلب',
        days: [
            { day: 'الأحد', from: '09:00 ص', to: '01:00 م', slots: ['09:00 ص', '10:00 ص', '11:00 ص', '12:00 م'] },
            { day: 'الإثنين', from: '09:00 ص', to: '02:00 م', slots: ['09:00 ص', '10:00 ص', '11:00 ص', '12:00 م', '01:00 م'] }
        ]
    },
    {
        id: 'mohamed',
        name: 'د. محمد علي',
        specialty: 'مخ وأعصاب',
        days: [
            { day: 'الإثنين', from: '10:00 ص', to: '04:00 م', slots: ['10:00 ص', '11:00 ص', '12:00 م', '01:00 م', '02:00 م', '03:00 م'] },
            { day: 'الأربعاء', from: '11:00 ص', to: '03:00 م', slots: ['11:00 ص', '12:00 م', '01:00 م', '02:00 م'] }
        ]
    },
    {
        id: 'rana',
        name: 'د. رنا سعد',
        specialty: 'أطفال',
        days: [
            { day: 'السبت', from: '02:00 م', to: '06:00 م', slots: ['02:00 م', '03:00 م', '04:00 م', '05:00 م'] },
            { day: 'الثلاثاء', from: '10:00 ص', to: '01:00 م', slots: ['10:00 ص', '11:00 ص', '12:00 م'] }
        ]
    }
];

function getDoctorsData() {
    const raw = localStorage.getItem('doctorSchedules');
    if (!raw) return JSON.parse(JSON.stringify(doctorsData));
    try {
        const parsed = JSON.parse(raw);
        if (Array.isArray(parsed) && parsed.length > 0) return parsed;
    } catch {
        // ignore and fallback
    }
    return JSON.parse(JSON.stringify(doctorsData));
}

function saveDoctorsData(data) {
    localStorage.setItem('doctorSchedules', JSON.stringify(data));
}

// goTo always navigates to an ABSOLUTE site path (must start with "/").
// This guarantees navigation never resolves relative to the current page.
function goTo(page) {
    window.location.href = page;
}

function goBackFromDoctorsSchedules() {
    const role = localStorage.getItem('userRole');
    if (role === 'manager') {
        goTo('/Manager/Dashboard');
        return;
    }

    if (role === 'secretary') {
        goTo('/Secretary/Dashboard');
        return;
    }

    goTo('/Home/Index');
}

function resolveDoctorIdFromIdentity(identity) {
    const id = (identity || '').toLowerCase();
    const doctors = getDoctorsData();
    const direct = doctors.find((d) => id.includes(d.id.toLowerCase()));
    if (direct) return direct.id;

    if (id.includes('john') || id.includes('جون')) return doctors.find((d) => d.id === 'john')?.id;
    if (id.includes('sara') || id.includes('سارة')) return doctors.find((d) => d.id === 'sara')?.id;
    if (id.includes('mohamed') || id.includes('محمد')) return doctors.find((d) => d.id === 'mohamed')?.id;
    if (id.includes('rana') || id.includes('رنا')) return doctors.find((d) => d.id === 'rana')?.id;

    return doctors[0]?.id;
}

function getCurrentDoctor() {
    const doctors = getDoctorsData();
    const currentId = localStorage.getItem('currentDoctorId');
    const doctor = doctors.find((d) => d.id === currentId);
    return doctor || doctors[0] || null;
}

function getStoredProfile() {
    return {
        name: localStorage.getItem('userName') || 'مستخدم جديد',
        phone: localStorage.getItem('userPhone') || '01000000000',
        email: localStorage.getItem('userEmail') || 'user@clinic.com'
    };
}

function handleRegister(event) {
    event.preventDefault();
    const name = document.getElementById('regName')?.value.trim();
    const phone = document.getElementById('regPhone')?.value.trim();
    const email = document.getElementById('regEmail')?.value.trim();
    const pass = document.getElementById('regPassword')?.value;
    const confirm = document.getElementById('regConfirmPassword')?.value;
    const msg = document.getElementById('registerMsg');

    if (!name || !phone || !email || !pass || !confirm) {
        if (msg) msg.textContent = 'من فضلك أدخل جميع البيانات.';
        return;
    }

    if (pass !== confirm) {
        if (msg) msg.textContent = 'كلمة المرور غير متطابقة.';
        return;
    }

    localStorage.setItem('userRole', 'user');
    localStorage.setItem('userName', name);
    localStorage.setItem('userPhone', phone);
    localStorage.setItem('userEmail', email);
    goTo('/Patient/Details');
}

function handleLogin(event) {
    event.preventDefault();
    const identity = (document.getElementById('loginIdentity')?.value || '').toLowerCase();
    const password = document.getElementById('loginPassword')?.value;
    const msg = document.getElementById('loginMsg');

    if (!identity || !password) {
        if (msg) msg.textContent = 'من فضلك أدخل البريد/اسم المستخدم وكلمة المرور.';
        return;
    }

    let role = 'user';
    if (identity.includes('manager')) role = 'manager';
    else if (identity.includes('secretary')) role = 'secretary';
    else if (identity.includes('doctor')) role = 'doctor';

    localStorage.setItem('userRole', role);
    if (identity.includes('@')) {
        localStorage.setItem('userEmail', identity);
    }

    if (role === 'doctor') {
        const doctorId = resolveDoctorIdFromIdentity(identity);
        if (doctorId) {
            localStorage.setItem('currentDoctorId', doctorId);
        }
    }

    if (role === 'manager') return goTo('/Manager/Dashboard');
    if (role === 'secretary') return goTo('/Secretary/Dashboard');
    if (role === 'doctor') return goTo('/Doctor/Dashboard');
    goTo('/Patient/Dashboard');
}

function logout() {
    localStorage.removeItem('userRole');
    localStorage.removeItem('currentDoctorId');
    goTo('/Home/Index');
}

function fillProfileFields() {
    const profile = getStoredProfile();
    const name = document.querySelectorAll('[data-user-name]');
    const phone = document.querySelectorAll('[data-user-phone]');
    const email = document.querySelectorAll('[data-user-email]');

    name.forEach((el) => { el.textContent = profile.name; });
    phone.forEach((el) => { el.textContent = profile.phone; });
    email.forEach((el) => { el.textContent = profile.email; });
}

function getReservations() {
    const data = localStorage.getItem('reservations');
    if (!data) {
        return [
            {
                patient: getStoredProfile().name,
                phone: getStoredProfile().phone,
                doctor: 'د. سارة جونسون',
                day: 'الإثنين',
                time: '10:00 ص',
                source: 'online',
                dateISO: new Date().toISOString().slice(0, 10)
            }
        ];
    }
    try {
        const list = JSON.parse(data);
        if (!Array.isArray(list)) return [];
        return list.map((r) => ({
            ...r,
            dateISO: r.dateISO || new Date().toISOString().slice(0, 10)
        }));
    } catch {
        return [];
    }
}

function saveReservation(item) {
    const list = getReservations();
    const normalized = {
        ...item,
        dateISO: item.dateISO || new Date().toISOString().slice(0, 10)
    };
    list.unshift(normalized);
    localStorage.setItem('reservations', JSON.stringify(list));
}

function renderReservationHistory() {
    const target = document.getElementById('userReservationHistory');
    if (!target) return;
    const list = getReservations();
    target.innerHTML = list.slice(0, 5).map((r) => `<tr><td>${r.doctor}</td><td>${r.day}</td><td>${r.time}</td><td><span class="badge">${r.source === 'online' ? 'أونلاين' : 'سكرتير'}</span></td></tr>`).join('');
}

function fillSecretaryBookings() {
    const target = document.getElementById('bookingListBody');
    if (!target) return;
    const list = getReservations();
    target.innerHTML = list.slice(0, 8).map((r) => `<tr><td>${r.patient}</td><td>${r.phone}</td><td>${r.doctor}</td><td>${r.day}</td><td>${r.time}</td><td><span class="badge">${r.source === 'online' ? 'حجز أونلاين' : 'حجز يدوي'}</span></td></tr>`).join('');

    const count = document.getElementById('reservationCounter');
    if (count) count.textContent = String(list.length);
}

function renderDoctorsList(targetId) {
    const target = document.getElementById(targetId);
    if (!target) return;
    const allDoctors = getDoctorsData();
    target.innerHTML = allDoctors.map((d) => {
        const days = d.days.map((x) => `<li>${x.day}: ${x.from} - ${x.to}</li>`).join('');
        return `<div class="card"><h4>${d.name}</h4><p class="muted">${d.specialty}</p><ul class="list">${days}</ul></div>`;
    }).join('');
}

function getTodayArabicName() {
    const map = ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'];
    return map[new Date().getDay()];
}

function renderTodayDoctorSchedules() {
    const target = document.getElementById('todayDoctorSchedulesBody');
    if (!target) return;

    const today = getTodayArabicName();
    const rows = [];

    const allDoctors = getDoctorsData();
    allDoctors.forEach((doctor) => {
        const shifts = doctor.days.filter((d) => d.day === today);
        if (shifts.length === 0) {
            rows.push(`<tr><td>${doctor.name}</td><td>${doctor.specialty}</td><td>${today}</td><td>-</td><td>-</td><td>غير متاح اليوم</td></tr>`);
            return;
        }

        shifts.forEach((shift) => {
            rows.push(`<tr><td>${doctor.name}</td><td>${doctor.specialty}</td><td>${shift.day}</td><td>${shift.from}</td><td>${shift.to}</td><td><span class="badge">متاح</span></td></tr>`);
        });
    });

    target.innerHTML = rows.join('');
}

function toggleManagerScheduleActions() {
    const managerActions = document.getElementById('managerScheduleActions');
    if (!managerActions) return;
    const role = localStorage.getItem('userRole');
    managerActions.style.display = role === 'manager' ? 'flex' : 'none';
}

function renderManagerDashboardStats() {
    const totalUsersEl = document.getElementById('statTotalUsers');
    const totalDoctorsEl = document.getElementById('statTotalDoctors');
    const totalSecretariesEl = document.getElementById('statTotalSecretaries');
    const weeklyReservationsEl = document.getElementById('reportWeeklyReservations');
    const occupancyEl = document.getElementById('reportOccupancy');
    const doctorsTodayEl = document.getElementById('reportDoctorsToday');

    if (!totalUsersEl || !totalDoctorsEl || !totalSecretariesEl) return;

    const reservations = getReservations();
    const allDoctors = getDoctorsData();
    const today = getTodayArabicName();

    const uniquePatients = new Set(
        reservations.map((r) => `${r.patient || ''}|${r.phone || ''}`)
    ).size;

    const totalDoctors = allDoctors.length;
    const totalSecretaries = Number(localStorage.getItem('secretariesCount') || 4);
    const totalUsers = 60 + uniquePatients;

    const doctorsToday = allDoctors.filter((doctor) => doctor.days.some((d) => d.day === today)).length;
    const availableSlotsToday = allDoctors.reduce((sum, doctor) => {
        const shiftsToday = doctor.days.filter((d) => d.day === today);
        return sum + shiftsToday.reduce((s, shift) => s + (Array.isArray(shift.slots) ? shift.slots.length : 0), 0);
    }, 0);

    const bookedToday = reservations.filter((r) => r.day === today).length;
    const occupancy = availableSlotsToday > 0
        ? Math.min(100, Math.round((bookedToday / availableSlotsToday) * 100))
        : 0;

    totalUsersEl.textContent = String(totalUsers);
    totalDoctorsEl.textContent = String(totalDoctors);
    totalSecretariesEl.textContent = String(totalSecretaries);

    if (weeklyReservationsEl) weeklyReservationsEl.textContent = String(reservations.length);
    if (occupancyEl) occupancyEl.textContent = `${occupancy}%`;
    if (doctorsTodayEl) doctorsTodayEl.textContent = String(doctorsToday);
}

function renderDoctorDashboard() {
    const nameEl = document.getElementById('doctorNameHeader');
    const summaryEl = document.getElementById('doctorSummaryHeader');
    const totalEl = document.getElementById('doctorTodayAppointments');
    const confirmedEl = document.getElementById('doctorConfirmedAppointments');
    const availEl = document.getElementById('doctorAvailableTime');
    const calendarEl = document.getElementById('doctorCalendarGrid');
    const appointmentsBody = document.getElementById('doctorAppointmentsBody');

    if (!nameEl || !summaryEl || !totalEl || !confirmedEl || !availEl || !calendarEl || !appointmentsBody) return;

    const doctor = getCurrentDoctor();
    if (!doctor) {
        nameEl.textContent = 'لا يوجد طبيب';
        summaryEl.textContent = 'لا توجد بيانات متاحة.';
        appointmentsBody.innerHTML = '<tr><td colspan="4" class="muted">لا توجد بيانات.</td></tr>';
        return;
    }

    nameEl.textContent = doctor.name;
    const daysText = doctor.days.map((d) => d.day).join(' - ');
    summaryEl.textContent = `الأيام: ${daysText} | التخصص: ${doctor.specialty}`;

    calendarEl.innerHTML = doctor.days.map((d) => `
        <div class="day-card"><strong>${d.day}</strong><p class="muted">${d.from} - ${d.to}</p></div>
    `).join('');

    const reservations = getReservations().filter((r) => (r.doctor || '').trim() === doctor.name);
    const today = getTodayArabicName();
    const todayReservations = reservations.filter((r) => r.day === today);

    const slotsToday = doctor.days
        .filter((d) => d.day === today)
        .reduce((sum, d) => sum + (Array.isArray(d.slots) ? d.slots.length : 0), 0);

    const availableSlots = Math.max(0, slotsToday - todayReservations.length);
    totalEl.textContent = String(todayReservations.length);
    confirmedEl.textContent = String(todayReservations.length);
    availEl.textContent = `${availableSlots * 0.5}h`;

    if (reservations.length === 0) {
        appointmentsBody.innerHTML = '<tr><td colspan="4" class="muted">لا توجد حجوزات لهذا الطبيب حتى الآن.</td></tr>';
        return;
    }

    appointmentsBody.innerHTML = reservations.slice(0, 8).map((r) => `
        <tr>
            <td>${r.patient || '-'}</td>
            <td>${r.day || '-'}</td>
            <td>${r.time || '-'}</td>
            <td><span class="badge">مؤكد</span></td>
        </tr>
    `).join('');
}

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
    specialtySelect.innerHTML = '<option value="">اختر التخصص</option>' + specialties.map((s) => `<option value="${s}">${s}</option>`).join('');

    function fillDoctors() {
        const selectedSpecialty = specialtySelect.value;
        const filtered = selectedSpecialty ? allDoctors.filter((d) => d.specialty === selectedSpecialty) : allDoctors;
        doctorSelect.innerHTML = '<option value="">اختر الطبيب</option>' + filtered.map((d) => `<option value="${d.id}">${d.name}</option>`).join('');
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

        availability.innerHTML = doctor.days.map((day, dayIndex) => {
            const slots = day.slots.map((slot, i) => `<button type="button" class="slot-btn" data-day="${day.day}" data-time="${slot}" data-doctor="${doctor.name}" data-idx="${dayIndex}-${i}">${slot}</button>`).join('');
            return `<div class="schedule-box"><strong>${day.day}</strong> <span class="muted">(${day.from} - ${day.to})</span><div class="slot-grid">${slots}</div></div>`;
        }).join('');

        availability.querySelectorAll('.slot-btn').forEach((btn) => {
            btn.addEventListener('click', () => {
                availability.querySelectorAll('.slot-btn').forEach((x) => x.classList.remove('active'));
                btn.classList.add('active');
                selected = {
                    doctor: btn.dataset.doctor,
                    day: btn.dataset.day,
                    time: btn.dataset.time
                };
                reservationMsg.textContent = `تم اختيار: ${selected.doctor} - ${selected.day} - ${selected.time}`;
                bookBtn.disabled = false;
            });
        });
    }

    specialtySelect.addEventListener('change', fillDoctors);
    doctorSelect.addEventListener('change', renderAvailability);
    fillDoctors();

    bookBtn.addEventListener('click', () => {
        if (!selected) return;
        const profile = getStoredProfile();
        const item = {
            patient: profile.name,
            phone: profile.phone,
            doctor: selected.doctor,
            day: selected.day,
            time: selected.time,
            source: 'online'
        };
        saveReservation(item);
        localStorage.setItem('lastReservation', JSON.stringify(item));
        goTo('/Reservation/Info');
    });
}

function fillReservationSummary() {
    const data = localStorage.getItem('lastReservation');
    if (!data) return;
    let item;
    try {
        item = JSON.parse(data);
    } catch {
        return;
    }
    const doctor = document.getElementById('resDoctor');
    const day = document.getElementById('resDay');
    const time = document.getElementById('resTime');
    if (doctor) doctor.textContent = item.doctor;
    if (day) day.textContent = item.day;
    if (time) time.textContent = item.time;
}

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

    if (!input.value) {
        input.value = new Date().toISOString().slice(0, 10);
    }

    const selectedDate = input.value;
    const reservations = getReservations().filter((r) => r.dateISO === selectedDate);

    if (reservations.length === 0) {
        body.innerHTML = '<tr><td colspan="6" class="muted">لا توجد حجوزات في هذا التاريخ.</td></tr>';
        return;
    }

    body.innerHTML = reservations.map((r) => `
        <tr>
            <td>${r.patient || '-'}</td>
            <td>${r.phone || '-'}</td>
            <td>${r.doctor || '-'}</td>
            <td>${r.day || '-'}</td>
            <td>${r.time || '-'}</td>
            <td><span class="badge">${r.source === 'online' ? 'أونلاين' : 'يدوي'}</span></td>
        </tr>
    `).join('');
}

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
    const id = `doc_${Date.now()}`;
    list.push({
        id,
        name,
        specialty,
        days: [{ day, from, to, slots }]
    });

    saveDoctorsData(list);
    if (msg) msg.textContent = 'تم إضافة الطبيب وجدوله بنجاح.';
    setTimeout(() => goTo('/Schedule/Index'), 700);
}

function initEditSchedulePage() {
    const select = document.getElementById('editDoctorSelect');
    if (!select) return;

    const list = getDoctorsData();
    select.innerHTML = '<option value="">اختر الطبيب</option>' + list.map((d) => `<option value="${d.id}">${d.name} - ${d.specialty}</option>`).join('');
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
    if (!doctor) {
        if (msg) msg.textContent = 'الطبيب غير موجود.';
        return;
    }

    const existingDay = doctor.days.find((d) => d.day === day);
    if (existingDay) {
        existingDay.from = from;
        existingDay.to = to;
        existingDay.slots = slots;
    } else {
        doctor.days.push({ day, from, to, slots });
    }

    saveDoctorsData(list);
    if (msg) msg.textContent = 'تم تحديث جدول الطبيب بنجاح.';
    setTimeout(() => goTo('/Schedule/Index'), 700);
}

function deleteDoctorDaySchedule() {
    const doctorId = document.getElementById('editDoctorSelect')?.value;
    const day = document.getElementById('editDoctorDay')?.value;
    const msg = document.getElementById('editScheduleMsg');

    if (!doctorId || !day) {
        if (msg) msg.textContent = 'اختر الطبيب واليوم أولاً.';
        return;
    }

    const list = getDoctorsData();
    const doctorIndex = list.findIndex((d) => d.id === doctorId);
    if (doctorIndex === -1) {
        if (msg) msg.textContent = 'الطبيب غير موجود.';
        return;
    }

    const beforeCount = list[doctorIndex].days.length;
    list[doctorIndex].days = list[doctorIndex].days.filter((d) => d.day !== day);

    if (list[doctorIndex].days.length === beforeCount) {
        if (msg) msg.textContent = 'لا يوجد ميعاد لهذا اليوم ليتم حذفه.';
        return;
    }

    if (list[doctorIndex].days.length === 0) {
        list.splice(doctorIndex, 1);
    }

    saveDoctorsData(list);
    if (msg) msg.textContent = 'تم حذف الميعاد بنجاح.';
    setTimeout(() => goTo('/Schedule/Index'), 700);
}

function deleteDoctorCompletely() {
    const doctorId = document.getElementById('editDoctorSelect')?.value;
    const msg = document.getElementById('editScheduleMsg');

    if (!doctorId) {
        if (msg) msg.textContent = 'اختر الطبيب أولاً.';
        return;
    }

    const list = getDoctorsData();
    const doctor = list.find((d) => d.id === doctorId);
    if (!doctor) {
        if (msg) msg.textContent = 'الطبيب غير موجود.';
        return;
    }

    const confirmed = window.confirm(`هل تريد حذف الطبيب ${doctor.name} بالكامل؟`);
    if (!confirmed) return;

    const updated = list.filter((d) => d.id !== doctorId);
    saveDoctorsData(updated);
    if (msg) msg.textContent = 'تم حذف الطبيب بالكامل.';
    setTimeout(() => goTo('/Schedule/Index'), 700);
}

window.addEventListener('DOMContentLoaded', () => {
    fillProfileFields();
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
        setInterval(renderManagerDashboardStats, 10000);
    }
});