const page = document.querySelector("main[data-page]");
const statusMessage = document.querySelector("#load-status");
const errorMessage = document.querySelector("#api-error");
const retryButton = document.querySelector("#retry-button");

async function loadData() {
    statusMessage.hidden = false;
    statusMessage.textContent = "Conectando con la API...";
    errorMessage.hidden = true;

    const endpoint = page.dataset.page === "personal"
        ? "/api/datospersonales"
        : "/api/mascotas";

    try {
        const response = await fetch(endpoint);
        if (!response.ok) {
            throw new Error(`La API respondió con estado ${response.status}`);
        }

        const data = await response.json();
        if (page.dataset.page === "personal") {
            showPersonalData(data);
        } else {
            showPets(data);
        }

        statusMessage.hidden = true;
    } catch {
        statusMessage.hidden = true;
        errorMessage.hidden = false;
    }
}

function showPersonalData(data) {
    for (const [field, value] of Object.entries(data)) {
        const output = document.querySelector(`[data-field="${field}"]`);
        if (output) {
            output.textContent = value;
        }
    }
}

function showPets(pets) {
    const tableBody = document.querySelector("#pets-table");
    tableBody.replaceChildren();

    for (const pet of pets) {
        const row = document.createElement("tr");
        for (const value of [pet.id, pet.nombre, pet.edad, pet.raza]) {
            const cell = document.createElement("td");
            cell.textContent = value;
            row.append(cell);
        }
        tableBody.append(row);
    }
}

if (page.dataset.page === "cursos") {
    const courseForm = document.querySelector("#course-form");
    const courseTable = document.querySelector("#courses-table");
    const courseStatus = document.querySelector("#courses-status");
    const courseListError = document.querySelector("#courses-error");
    const courseSuccess = document.querySelector("#course-success");
    const courseError = document.querySelector("#course-error");

    async function loadCourses() {
        courseStatus.hidden = false;
        courseStatus.textContent = "Cargando cursos...";
        courseListError.hidden = true;

        try {
            const response = await fetch("/api/cursos");
            if (!response.ok) {
                throw new Error(`La API respondió con estado ${response.status}`);
            }

            const courses = await response.json();
            courseTable.replaceChildren();

            if (courses.length === 0) {
                const row = document.createElement("tr");
                const cell = document.createElement("td");
                cell.className = "table-placeholder";
                cell.colSpan = 8;
                cell.textContent = "No hay cursos registrados.";
                row.append(cell);
                courseTable.append(row);
            }

            for (const course of courses) {
                const row = document.createElement("tr");
                for (const value of [
                    course.id,
                    course.codigo,
                    course.nombre,
                    course.area,
                    course.horas,
                    course.cupoMaximo,
                    course.fechaInicio
                ]) {
                    const cell = document.createElement("td");
                    cell.textContent = value;
                    row.append(cell);
                }

                const actionsCell = document.createElement("td");
                const deleteButton = document.createElement("button");
                deleteButton.className = "course-delete-button";
                deleteButton.type = "button";
                deleteButton.textContent = "Eliminar";
                deleteButton.addEventListener("click", () => deleteCourse(course.id));
                actionsCell.append(deleteButton);
                row.append(actionsCell);
                courseTable.append(row);
            }

            courseStatus.hidden = true;
        } catch {
            courseStatus.hidden = true;
            courseListError.textContent = "No se pudieron cargar los cursos. Revisa que la API esté ejecutándose.";
            courseListError.hidden = false;
        }
    }

    async function deleteCourse(id) {
        courseError.hidden = true;
        courseSuccess.hidden = true;

        try {
            const response = await fetch(`/api/cursos/${id}`, { method: "DELETE" });
            if (!response.ok) {
                throw new Error(`La API respondió con estado ${response.status}`);
            }

            courseSuccess.textContent = "Curso eliminado correctamente.";
            courseSuccess.hidden = false;
            await loadCourses();
        } catch {
            courseError.textContent = "No se pudo eliminar el curso. Inténtalo de nuevo.";
            courseError.hidden = false;
        }
    }

    courseForm.addEventListener("submit", async (event) => {
        event.preventDefault();
        courseSuccess.hidden = true;
        courseError.hidden = true;

        const formData = new FormData(courseForm);
        const course = {
            codigo: formData.get("codigo").trim(),
            nombre: formData.get("nombre").trim(),
            area: formData.get("area").trim(),
            horas: Number(formData.get("horas")),
            cupoMaximo: Number(formData.get("cupoMaximo")),
            fechaInicio: formData.get("fechaInicio")
        };

        try {
            const response = await fetch("/api/cursos", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(course)
            });

            if (!response.ok) {
                throw new Error(`La API respondió con estado ${response.status}`);
            }

            courseForm.reset();
            courseSuccess.textContent = "Curso creado correctamente.";
            courseSuccess.hidden = false;
            await loadCourses();
        } catch {
            courseError.textContent = "No se pudo crear el curso. Revisa los datos e inténtalo de nuevo.";
            courseError.hidden = false;
        }
    });

    loadCourses();
} else if (page.dataset.page === "sumadora") {
    const sumForm = document.querySelector("#sum-form");
    const sumResult = document.querySelector("#sum-result");
    const sumError = document.querySelector("#sum-error");

    sumForm.addEventListener("submit", async (event) => {
        event.preventDefault();
        sumResult.hidden = true;
        sumError.hidden = true;

        const numero1 = sumForm.elements.numero1.valueAsNumber;
        const numero2 = sumForm.elements.numero2.valueAsNumber;
        if (!Number.isFinite(numero1) || !Number.isFinite(numero2)) {
            sumError.textContent = "Ingresa dos números válidos.";
            sumError.hidden = false;
            return;
        }

        try {
            const response = await fetch("/api/sumadora", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ numero1, numero2 })
            });

            if (!response.ok) {
                throw new Error("La API no pudo realizar la suma.");
            }

            const data = await response.json();
            sumResult.textContent = `Resultado: ${data.resultado}`;
            sumResult.hidden = false;
        } catch {
            sumError.textContent = "No se pudo conectar con la API. Inténtalo de nuevo.";
            sumError.hidden = false;
        }
    });
} else {
    retryButton.addEventListener("click", loadData);
    loadData();
}