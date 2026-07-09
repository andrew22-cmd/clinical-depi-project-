// =======================
// Initialization
// =======================
document.addEventListener("DOMContentLoaded", () => {

    if (document.getElementById("usersTableBody")) {
        loadUsersTable();
    }

    if (document.getElementById("scheduleDoctorSelect")) {
        loadDoctorsForSelect();
    }
    if (document.getElementById("doctorSchedulesGrid")) {

        loadDoctorSchedules();

    }

    if (document.getElementById("doctorScheduleBody")) {

        loadDoctorsSchedulesPatient();

    }
    if (document.getElementById("patientName")) {
        loadPatientsForNotes();
    }
    if (document.getElementById("schedulesTableBody")) {
        renderSchedulesTable();
    }
    if (document.getElementById("doctorNameHeader")) {
        loadDoctorInfo();
    }
    if (document.getElementById("doctorCalendarGrid")) {
        loadWeeklySchedule();
    }
    if (document.querySelector("[data-user-name]")) {
        loadPatientInfo();
    }
    if (document.getElementById("specialtySelect")) {
        loadSpecialities();

    }
    if (document.getElementById("allDoctorsSchedules")) {
        loadDoctorSchedules();
    }
    if (document.getElementById("reservationCounter")) {

        loadSecretaryStatistics();

    }
    if (document.getElementById("todayPatientsBody")) {
        loadTodayPatients();
    }
    if (document.getElementById("userReservationHistory")) {
        loadReservationHistory();
    }
    document.getElementById("specialtySelect")
        ?.addEventListener("change", loadDoctorsBySpeciality);


    const bookBtn = document.getElementById("bookBtn");

    if (bookBtn) {
        bookBtn.addEventListener("click", bookReservation);
    }

    const doctorSelect = document.getElementById("doctorSelect");
    const reservationDate = document.getElementById("reservationDate");

    if (doctorSelect) {

        doctorSelect.addEventListener("change", loadDoctorSlots);

    }

    if (reservationDate) {

        reservationDate.addEventListener("change", loadDoctorSlots);

    }
    
    if (document.getElementById("secSpeciality")) {

        loadSecretarySpecialities();

        document.getElementById("secSpeciality")
            .addEventListener("change", loadSecretaryDoctors);

    }
    if (document.getElementById("patientCheckupsHistory")) {

        loadPatientCheckups();

    }
    const secDoctor = document.getElementById("secDoctor");

    if (secDoctor) {

        secDoctor.addEventListener("change", function () {

            loadSecretarySlots(this.value);

        });

        document.getElementById("secReservationDate")
            ?.addEventListener("change", function () {
                const doctorId = document.getElementById("secDoctor").value;
                if (doctorId) loadSecretarySlots(doctorId);
            });

        if (document.getElementById("bookingListBody")) {

            loadReservationTable();

        }


    }


});

// =======================
// Load Users
// =======================
async function loadUsersTable() {

    const tableBody = document.getElementById("usersTableBody");
    if (!tableBody) return;

    try {

        const response = await fetch("/Manager/GetAllUsersList");
        let data = await response.json();
        if(data.message) {
            if(data.success) toastr.success(data.message);
            else toastr.error(data.message);
        }
        if (data.data) data = data.data;

        tableBody.innerHTML = "";

        data.forEach(user => {

            const row = document.createElement("tr");

            row.innerHTML = `
    <td>${user.name}</td>
    <td>${user.email}</td>
    <td>${user.phone}</td>
    <td>${user.role}</td>
    <td>${user.status}</td>
    <td>
        <button class="btn btn-danger btn-sm"
                onclick="deleteUser(${user.id}, '${user.role}')">
            <i class="fas fa-trash"></i>
        </button>
    </td>
`;

            tableBody.appendChild(row);

        });

    }
    catch (err) {

        console.error(err);

        tableBody.innerHTML =
            "<tr><td colspan='5'>خطأ في تحميل المستخدمين</td></tr>";

    }

}

// =======================
// Load Doctors
// =======================
async function loadDoctorsForSelect() {

    const select = document.getElementById("scheduleDoctorSelect");

    if (!select) return;

    try {

        const response = await fetch("/Manager/GetDoctorsListOnly");
        let doctors = await response.json();
        if(doctors.message) {
            if(doctors.success) toastr.success(doctors.message);
            else toastr.error(doctors.message);
        }
        if (doctors.data) doctors = doctors.data;

        select.innerHTML = `<option value="">اختر الطبيب</option>`;

        doctors.forEach(doc => {

            const option = document.createElement("option");

            option.value = doc.id;
            option.textContent = doc.name;

            select.appendChild(option);

        });

    }
    catch (err) {

        console.error(err);

    }

}

// =======================
// Save Schedule
// =======================
async function saveDoctorSchedule(event) {

    if (event)
        event.preventDefault();

    const payload = {

        DoctorId: parseInt(document.getElementById("scheduleDoctorSelect").value),
        Day: document.getElementById("scheduleDay").value,
        StartTime: document.getElementById("scheduleStartTime").value,
        EndTime: document.getElementById("scheduleEndTime").value

    };

    try {

        const response = await fetch("/Manager/SaveSchedule", {

            method: "POST",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify(payload)

        });

        const result = await response.json();

        if (response.ok && result.success !== false) {
            toastr.success(result.message || "تم الحفظ");
            renderSchedulesTable();
        } else {
            toastr.error(result.message || "حدث خطأ");
        }

    }
    catch (err) {
        console.error(err);
        toastr.error("حدث خطأ أثناء الحفظ");
    }

}

// =======================
// Render Schedules
// =======================
async function renderSchedulesTable() {
    const tableBody = document.getElementById("schedulesTableBody");
    if (!tableBody) return;

    try {
        const response = await fetch('/Manager/GetSchedules');
        let schedules = await response.json();
        if(schedules.message) {
            if(schedules.success) toastr.success(schedules.message);
            else toastr.error(schedules.message);
        }
        if (schedules.data) schedules = schedules.data;

        tableBody.innerHTML = "";

        schedules.forEach(sch => {

            let row = document.createElement("tr");

            row.innerHTML = `
                <td>${sch.doctorName}</td>
                <td><span class="badge bg-primary">${sch.day}</span></td>
                <td>${sch.startTime}</td>
                <td>${sch.endTime}</td>
                <td>
                    <button class="btn btn-danger btn-sm"
                        onclick="deleteSchedule(${sch.id})">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            `;

            tableBody.appendChild(row);
        });

    } catch (err) {
        console.error(err);
        tableBody.innerHTML =
            "<tr><td colspan='5'>خطأ في تحميل البيانات</td></tr>";
    }
}

async function deleteSchedule(id) {

    if (!confirm("هل تريد حذف هذا الميعاد؟"))
        return;

    const response = await fetch(`/Manager/DeleteSchedule/${id}`, {
        method: "DELETE"
    });

    const result = await response.json();

    if (result.success) {
        toastr.success(result.message || "تم الحذف");
        renderSchedulesTable();
    } else {
        toastr.error(result.message || "حدث خطأ");
    }
}
async function deleteUser(id, role) {

    if (!confirm("هل أنت متأكد من حذف هذا المستخدم؟"))
        return;

    try {

        const response = await fetch(`/Manager/DeleteStaff?id=${id}&role=${encodeURIComponent(role)}`, {
            method: "DELETE"
        });

        if (response.ok) {
            toastr.success("تم الحذف بنجاح");
            loadUsersTable();
        }
        else {
            toastr.error("فشل الحذف");
        }

    } catch (err) {
        console.error(err);
        toastr.error("حدث خطأ");
    }

}
async function loadDoctorInfo() {

    const response = await fetch("/Doctor/GetDoctorInfo");

    let data = await response.json();
    if (data.data) data = data.data;

    document.getElementById("doctorNameHeader").innerText = data.name;

    document.getElementById("doctorSummaryHeader").innerText = data.speciality;
}


async function loadWeeklySchedule() {

    const grid = document.getElementById("doctorCalendarGrid");

    const response = await fetch("/Doctor/GetWeeklySchedule");

    let schedules = await response.json();
    if(schedules.data) schedules = schedules.data;

    const days = [
        "السبت",
        "الأحد",
        "الإثنين",
        "الثلاثاء",
        "الأربعاء",
        "الخميس",
        "الجمعة"
    ];

    grid.innerHTML = "";

    days.forEach(day => {

        const daySchedules = schedules.filter(x => x.day === day);

        let html = `
            <div class="card">
                <h4>${day}</h4>
        `;

        if (daySchedules.length === 0) {

            html += `<p class="muted">لا توجد مواعيد</p>`;

        } else {

            daySchedules.forEach(item => {

                html += `
                    <div style="margin:.5rem 0;padding:.5rem;border-radius:8px;background:#eef5ff">
                        ${item.start} - ${item.end}
                    </div>
                `;

            });

        }

        html += `</div>`;

        grid.innerHTML += html;

    });

}
async function loadPatientInfo() {

    try {

        const response = await fetch("/Patient/GetPatientInfo");

        if (!response.ok)
            return;

        let data = await response.json();
        if (data.data) data = data.data;

        document.querySelectorAll("[data-user-name]").forEach(x => {
            x.innerText = data.name;
        });

        document.querySelectorAll("[data-user-phone]").forEach(x => {
            x.innerText = data.phone;
        });

        document.querySelectorAll("[data-user-email]").forEach(x => {
            x.innerText = data.email;
        });

    }
    catch (err) {
        console.error(err);
    }

}
// =======================
// Navigation
// =======================

function goTo(url) {
    window.location.href = url;
}

// =======================
// Edit Profile
// =======================

function toggleEditProfile() {

    const form = document.getElementById("editProfileForm");
    const view = document.getElementById("viewProfileData");

    if (!form || !view)
        return;

    if (form.style.display === "none" || form.style.display === "") {

        form.style.display = "block";
        view.style.display = "none";

    }
    else {

        form.style.display = "none";
        view.style.display = "block";

    }

}

// =======================
// Save Personal Data
// =======================


async function savePersonalData() {

    const data = {
        name: document.getElementById("editNameInput").value,
        phone: document.getElementById("editPhoneInput").value,
        password: document.getElementById("editPasswordInput").value
    };

    const response = await fetch("/Patient/UpdateProfile", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });

    const result = await response.json();

    if (result.success) {
        toastr.success(result.message);
        loadPatientInfo();
        toggleEditProfile();
    } else {
        toastr.error(result.message || "حدث خطأ");
    }
}
async function loadSpecialities() {

    const select = document.getElementById("specialtySelect");

    const response = await fetch("/Patient/GetSpecialities");

    let data = await response.json();
    if (data.data) data = data.data;

    select.innerHTML = `<option value="">اختر التخصص</option>`;

    data.forEach(item => {

        select.innerHTML += `
            <option value="${item.id}" data-fee="${item.fee}">
                ${item.name} &mdash; ${item.fee} ج.م
            </option>
        `;

    });

}
async function loadDoctorsBySpeciality() {

    const specialitySelect = document.getElementById("specialtySelect");
    const specialityId = specialitySelect.value;
    const selectedOption = specialitySelect.options[specialitySelect.selectedIndex];
    const fee = selectedOption?.getAttribute("data-fee");
    const doctorSelect = document.getElementById("doctorSelect");

    // عرض سعر التخصص
    const feeBadge = document.getElementById("specialityFeeBadge");
    if (feeBadge) {
        if (fee && specialityId) {
            feeBadge.style.display = "inline-block";
            feeBadge.textContent = `سعر الكشف: ${fee} ج.م`;
        } else {
            feeBadge.style.display = "none";
        }
    }

    doctorSelect.innerHTML = "<option>Loading...</option>";

    if (!specialityId) {
        doctorSelect.innerHTML =
            "<option value=''>اختر الطبيب</option>";
        document.getElementById("doctorAvailability").innerHTML = "";
        document.getElementById("bookBtn").disabled = true;
        selectedSlotId = null;
        return;
    }

    const response = await fetch(
        `/Patient/GetDoctorsBySpeciality?specialityId=${specialityId}`
    );

    let doctors = await response.json();
    if (doctors.data) doctors = doctors.data;

    doctorSelect.innerHTML =
        "<option value=''>اختر الطبيب</option>";

    doctors.forEach(doc => {

        doctorSelect.innerHTML += `
            <option value="${doc.id}">
                ${doc.name}
            </option>
        `;

    });

}
async function loadDoctorSlots() {

    const doctorId = document.getElementById("doctorSelect").value;
    const reservationDate = document.getElementById("reservationDate").value;

    const container = document.getElementById("doctorAvailability");

    if (!container)
        return;

    container.innerHTML = "";

    document.getElementById("bookBtn").disabled = true;
    selectedSlotId = null;

    if (!doctorId || !reservationDate)
        return;

    const response = await fetch(
        `/Patient/GetDoctorSlots?doctorId=${doctorId}&reservationDate=${reservationDate}`
    );

    let data = await response.json();
    if (data.data) data = data.data;

    data.forEach(day => {

        let html = `
            <div class="card" style="margin-top:15px">
                <h4>${day.day}</h4>
        `;

        if (day.slots.length === 0) {

            html += "<p>لا توجد مواعيد</p>";

        } else {

            day.slots.forEach(slot => {

                html += `
                    <button
                        type="button"
                        class="btn btn-outline slot-btn"
                        onclick="selectSlot(${slot.slotId}, this)"
                        style="margin:5px">
                        ${slot.time}
                    </button>
                `;

            });

        }

        html += "</div>";

        container.innerHTML += html;

    });

}
let selectedSlotId = null;

function selectSlot(slotId, btn) {

    selectedSlotId = slotId;

    document.querySelectorAll(".slot-btn").forEach(x => {

        x.classList.remove("btn-primary");
        x.classList.add("btn-outline");

    });

    btn.classList.remove("btn-outline");
    btn.classList.add("btn-primary");

    document.getElementById("bookBtn").disabled = false;

}
async function bookReservation() {

    if (!selectedSlotId) {
        alert("اختر موعد أولاً.");
        return;
    }

    const response = await fetch("/Patient/BookReservation", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            slotId: selectedSlotId,
            reservationDate: document.getElementById("reservationDate").value
        })
    });

    const result = await response.json();

    if (result.success) {
        toastr.success(result.message);
        document.getElementById("bookBtn").disabled = true;
        selectedSlotId = null;
        loadDoctorSlots();
    } else {
        toastr.error(result.message || "حدث خطأ");
    }
}
async function loadReservationHistory() {

    const tbody = document.getElementById("userReservationHistory");

    if (!tbody)
        return;

    const response = await fetch("/Patient/GetReservationHistory");

    let data = await response.json();
    if (data.data) data = data.data;

    tbody.innerHTML = "";

    if (data.length === 0) {

        tbody.innerHTML = `
            <tr>
                <td colspan="6">
                    لا توجد حجوزات.
                </td>
            </tr>
        `;

        return;
    }

    data.forEach(item => {

        const cancelBtn = item.hasNotes
            ? `<span style="font-size:0.8rem; color:#888;" title="تمت زيارة الطبيب وأضاف ملاحظات">&#128274; لا يمكن الإلغاء</span>`
            : item.status === "Cancelled"
                ? `<span class="badge bg-danger">Cancelled</span>`
                : `<button class="btn btn-danger btn-sm" onclick="cancelReservation(${item.reservationId})">إلغاء</button>`;

        tbody.innerHTML += `
            <tr>
                <td>${item.doctor}</td>
                <td>${item.speciality ?? ''}</td>
                <td>${item.date ?? ''}</td>
                <td>${item.day}</td>
                <td>${item.time}</td>
                <td>${item.source}</td>
                <td><strong>${item.consultationFee ?? ''} ج.م</strong></td>
                <td>${item.status}</td>
                <td>${cancelBtn}</td>
            </tr>
        `;

    });

}
async function cancelReservation(id) {

    if (!confirm("هل تريد إلغاء الحجز؟"))
        return;

    const response = await fetch(`/Patient/CancelReservation/${id}`, {
        method: "POST"
    });

    const result = await response.json();

    if (result.success) {
        toastr.success(result.message);
        await loadReservationTable();
        await loadSecretaryStatistics();
        await loadDoctorSchedules();
        const doctorId = document.getElementById("secDoctor")?.value;
        if (doctorId) {
            await loadSecretarySlots(doctorId);
        }
    } else {
        toastr.error(result.message || "حدث خطأ");
    }

}
async function loadDoctorSchedules() {

    const grid = document.getElementById("doctorSchedulesGrid");

    if (!grid) return;

    const response = await fetch("/Secretary/GetDoctorsSchedules");
    let data = await response.json();
    if (data.data) data = data.data;

    grid.innerHTML = "";

    const doctors = {};

    data.forEach(item => {

        if (!doctors[item.doctor]) {
            doctors[item.doctor] = [];
        }

        doctors[item.doctor].push(item);

    });

    Object.keys(doctors).forEach(doctor => {

        let schedules = "";

        doctors[doctor].forEach(item => {

            schedules += `
                <div class="schedule-item">
                    <span class="badge bg-primary">${item.day}</span>
                    <span>${item.start} - ${item.end}</span>
                </div>
            `;

        });

        grid.innerHTML += `
            <div class="doctor-card">
                <h4>👨‍⚕️ ${doctor}</h4>
                ${schedules}
            </div>
        `;

    });

}
async function loadSecretarySpecialities() {

    const select = document.getElementById("secSpeciality");

    const response = await fetch("/Secretary/GetSpecialities");

    let data = await response.json();
    if (data.data) data = data.data;

    select.innerHTML = `<option value="">اختر التخصص</option>`;

    data.forEach(item => {

        select.innerHTML += `
            <option value="${item.id}">
                ${item.name}
            </option>
        `;

    });

}
async function loadSecretaryDoctors() {

    const specialityId =
        document.getElementById("secSpeciality").value;

    const doctorSelect =
        document.getElementById("secDoctor");

    doctorSelect.innerHTML =
        "<option>Loading...</option>";

    if (!specialityId) {

        doctorSelect.innerHTML =
            "<option value=''>اختر الطبيب</option>";

        return;
    }

    const response = await fetch(
        `/Secretary/GetDoctorsBySpeciality?specialityId=${specialityId}`
    );

    let doctors = await response.json();
    if (doctors.data) doctors = doctors.data;

    doctorSelect.innerHTML =
        "<option value=''>اختر الطبيب</option>";

    doctors.forEach(doc => {

        doctorSelect.innerHTML += `
            <option value="${doc.id}">
                ${doc.name}
            </option>
        `;

    });

}
let selectedSecretarySlot = null;

async function loadSecretarySlots(doctorId) {

    const container = document.getElementById("secretarySlots");

    container.innerHTML = "";

    if (!doctorId)
        return;
    const reservationDate =
        document.getElementById("secReservationDate").value;

    const response =
        await fetch(`/Secretary/GetDoctorSlots?doctorId=${doctorId}&reservationDate=${reservationDate}`);

    let data = await response.json();
    if (data.data) data = data.data;

    data.forEach(day => {

        let html = `
            <div class="card" style="margin-top:15px">
                <h4>${day.day}</h4>
        `;

        if (day.slots.length == 0) {

            html += "<p>لا توجد مواعيد</p>";

        }
        else {

            day.slots.forEach(slot => {

                html += `
                   <button
    type="button"
    class="btn btn-outline sec-slot"
    onclick="selectSecretarySlot(${slot.slotId},this)"
    style="margin:5px">
                ${slot.time}
                </button>
            `;

            });

        }

        html += "</div>";

        container.innerHTML += html;

    });

}
function selectSecretarySlot(slotId, btn) {

    selectedSecretarySlot = slotId;

    document.querySelectorAll(".sec-slot").forEach(x => {

        x.classList.remove("btn-primary");
        x.classList.add("btn-outline");

    });

    btn.classList.remove("btn-outline");
    btn.classList.add("btn-primary");

}
async function createSecretaryReservation(e) {

    e.preventDefault();

    if (!selectedSecretarySlot) {
        toastr.warning("اختر موعداً أولاً.");
        return;
    }

    const doctorId = document.getElementById("secDoctor").value;
    if (!doctorId) {
        toastr.warning("اختر الطبيب أولاً.");
        return;
    }

    const data = {
        patientName: document.getElementById("secPatientName").value,
        patientPhone: document.getElementById("secPatientPhone").value,
        doctorId: parseInt(doctorId),
        slotId: selectedSecretarySlot,
        reservationDate: document.getElementById("secReservationDate").value
    };

    const response = await fetch("/Secretary/CreateReservation", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    const result = await response.json();

    if (result.success) {
        toastr.success(result.message);
        selectedSecretarySlot = null;
        document.getElementById("secPatientName").value = "";
        document.getElementById("secPatientPhone").value = "";
        document.getElementById("secretarySlots").innerHTML = "";
        await loadReservationTable();
        await loadSecretaryStatistics();
    } else {
        toastr.error(result.message || "حدث خطأ أثناء الحجز.");
    }

}
async function loadReservationTable() {

    const tbody = document.getElementById("bookingListBody");

    const response =
        await fetch("/Secretary/GetReservations");

    let data = await response.json();
    if (data.data) data = data.data;

    tbody.innerHTML = "";

    data.forEach(item => {

        tbody.innerHTML += `

<tr>

<td>${item.patient}</td>

<td>${item.phone}</td>

<td>${item.doctor}</td>

<td>${item.day}</td>

<td>${item.time}</td>

<td>${item.source}</td>

<td>

<button
class="btn btn-danger"
onclick="cancelReservation(${item.id})">

Cancel

</button>

</td>

</tr>

`;

    });

}
async function loadSecretaryStatistics() {

    const response =
        await fetch("/Secretary/GetDashboardStatistics");

    let data = await response.json();
    if (data.data) data = data.data;

    document.getElementById("reservationCounter").innerText =
        data.totalReservations;

    document.getElementById("todayReservationCounter").innerText =
        data.todayReservations;

}
async function loadTodayPatients() {

    const body = document.getElementById("todayPatientsBody");

    if (!body) return;

    const response = await fetch("/Doctor/GetTodayPatients");

    let data = await response.json();
    if (data.data) data = data.data;

    body.innerHTML = "";

    if (data.length === 0) {

        body.innerHTML = `
            <tr>
                <td colspan="3">
                    لا يوجد مرضى اليوم
                </td>
            </tr>
        `;

        return;
    }

    data.forEach(item => {

        body.innerHTML += `
            <tr>
                <td>${item.patient}</td>
                <td>${item.phone}</td>
                <td>${item.time}</td>
            </tr>
        `;

    });

}
async function loadPatientsForNotes() {

    const select = document.getElementById("patientName");

    if (!select)
        return;

    const response =
        await fetch("/Doctor/GetPatientsForNotes");

    let data = await response.json();
    if (data.data) data = data.data;

    select.innerHTML =
        `<option value="">اختر المريض</option>`;

    data.forEach(x => {

        select.innerHTML += `
            <option value="${x.reservationId}">
                ${x.patient}
            </option>
        `;

    });

}
async function saveDiagnosis(e) {

    e.preventDefault();

    const data = {

        reservationId: parseInt(document.getElementById("patientName").value),

        visitDate: document.getElementById("visitDate").value,

        diagnosis: document.getElementById("diagnosis").value,

        notes: document.getElementById("notes").value

    };

    if (!data.reservationId) {

        alert("اختر المريض");

        return;
    }

    const response = await fetch("/Doctor/SavePatientNote", {

        method: "POST",

        headers: {
            "Content-Type": "application/json"
        },

        body: JSON.stringify(data)

    });

    const result = await response.json();

    if (result.success) {
        toastr.success(result.message);
        document.getElementById("diagnosis").value = "";
        document.getElementById("notes").value = "";
        document.getElementById("visitDate").value = "";
        document.getElementById("patientName").selectedIndex = 0;
    } else {
        toastr.error(result.message || "حدث خطأ");
    }

}
async function loadPatientCheckups() {

    const div = document.getElementById("patientCheckupsHistory");

    if (!div)
        return;

    const response =
        await fetch("/Patient/GetPatientCheckups");

    let data = await response.json();
    if (data.data) data = data.data;

    div.innerHTML = "";

    if (data.length == 0) {

        div.innerHTML =
            "<p class='muted'>لا توجد تفاصيل كشوفات سابقة.</p>";

        return;
    }

    data.forEach(item => {

        div.innerHTML += `
<div class="card" style="margin-bottom:15px">

    <h3>${item.speciality}</h3>

    <small>${item.date}</small>

    <p><b>Diagnosis:</b> ${item.diagnosis}</p>

    <p><b>Notes:</b> ${item.notes}</p>

</div>
`;

    });

}
async function loadDoctorsSchedulesPatient() {

    const body = document.getElementById("doctorScheduleBody");

    if (!body)
        return;

    const response =
        await fetch("/Patient/GetDoctorsSchedules");

    let data = await response.json();
    if (data.data) data = data.data;

    body.innerHTML = "";

    data.forEach(item => {

        body.innerHTML += `

<tr>

<td>${item.doctor}</td>

<td>${item.speciality}</td>

<td>${item.day}</td>

<td>${item.start}</td>

<td>${item.end}</td>

</tr>

`;

    });

}
async function renderReservationsByDate() {

    const tbody = document.getElementById("reservationListByDateBody");

    if (!tbody) return;

    const date = document.getElementById("reservationDateFilter").value;

    const response = await fetch(`/Manager/GetReservationsByDate?date=${date}`);
    let data = await response.json();
    if (data.data) data = data.data;

    tbody.innerHTML = "";

    if (data.length === 0) {
        tbody.innerHTML = `
        <tr>
            <td colspan="6">لا توجد حجوزات في هذا اليوم.</td>
        </tr>`;
        return;
    }

    data.forEach(r => {

        tbody.innerHTML += `
        <tr>
            <td>${r.patient}</td>
            <td>${r.phone}</td>
            <td>${r.doctor}</td>
            <td>${r.day}</td>
            <td>${r.time}</td>
            <td>${r.source}</td>
        </tr>`;
    });

}
function setReservationFilterToday() {

    const input = document.getElementById("reservationDateFilter");

    const today = new Date().toISOString().split("T")[0];

    input.value = today;

    renderReservationsByDate();

}
function shiftReservationDate(days) {

    const input = document.getElementById("reservationDateFilter");

    let date = new Date(input.value);

    date.setDate(date.getDate() + days);

    input.value = date.toISOString().split("T")[0];

    renderReservationsByDate();

}