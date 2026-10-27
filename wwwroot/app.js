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
    const editDialog = document.querySelector("#edit-course-dialog");
    const editForm = document.querySelector("#edit-course-form");
    const editError = document.querySelector("#edit-course-error");
    let editingCourseId = null;

    function createApiError(message) {
        const error = new Error(message);
        error.name = "ApiError";
        return error;
    }

    async function getApiError(response, fallback) {
        try {
            const problem = await response.json();
            const validationErrors = Object.values(problem.errors ?? {}).flat();
            return validationErrors.length > 0
                ? validationErrors.join(" ")
                : problem.detail || problem.title || fallback;
        } catch {
            return fallback;
        }
    }

    function validateCourseForm(form, errorElement) {
        const fields = [
            {
                name: "codigo",
                label: "El código",
                requiredMessage: "El código es obligatorio.",
                maxLength: 30
            },
            {
                name: "nombre",
                label: "El nombre",
                requiredMessage: "El nombre es obligatorio.",
                maxLength: 120
            },
            {
                name: "area",
                label: "El área",
                requiredMessage: "El área es obligatoria.",
                maxLength: 80
            },
            {
                name: "horas",
                label: "La cantidad de horas",
                requiredMessage: "La cantidad de horas es obligatoria.",
                min: 1
            },
            {
                name: "cupoMaximo",
                label: "El cupo máximo",
                requiredMessage: "El cupo máximo es obligatorio.",
                min: 1
            },
            {
                name: "fechaInicio",
                label: "La fecha de inicio",
                requiredMessage: "La fecha de inicio es obligatoria."
            }
        ];

        for (const field of fields) {
            const input = form.elements[field.name];
            const value = input.value.trim();
            let message = "";

            if (!value) {
                message = field.requiredMessage;
            } else if (field.maxLength && value.length > field.maxLength) {
                message = `${field.label} no puede superar ${field.maxLength} caracteres.`;
            } else if (field.min && (!Number.isInteger(Number(value)) || Number(value) < field.min)) {
                message = `${field.label} debe ser un número entero mayor o igual a ${field.min}.`;
            } else if (field.name === "fechaInicio" && Number.isNaN(Date.parse(value))) {
                message = "La fecha de inicio no es válida.";
            }

            if (message) {
                errorElement.textContent = message;
                errorElement.hidden = false;
                input.focus();
                return false;
            }
        }

        errorElement.hidden = true;
        return true;
    }

    function getCourseFromForm(form) {
        const formData = new FormData(form);
        return {
            codigo: formData.get("codigo").trim(),
            nombre: formData.get("nombre").trim(),
            area: formData.get("area").trim(),
            horas: Number(formData.get("horas")),
            cupoMaximo: Number(formData.get("cupoMaximo")),
            fechaInicio: formData.get("fechaInicio")
        };
    }

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
                const editButton = document.createElement("button");
                editButton.className = "course-edit-button";
                editButton.type = "button";
                editButton.textContent = "Editar";
                editButton.addEventListener("click", () => openEditDialog(course));
                const deleteButton = document.createElement("button");
                deleteButton.className = "course-delete-button";
                deleteButton.type = "button";
                deleteButton.textContent = "Eliminar";
                deleteButton.addEventListener("click", () => deleteCourse(course.id));
                actionsCell.append(editButton, deleteButton);
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
                throw createApiError(await getApiError(response, "No se pudo eliminar el curso."));
            }

            courseSuccess.textContent = "Curso eliminado correctamente.";
            courseSuccess.hidden = false;
            await loadCourses();
        } catch (error) {
            courseError.textContent = error.name === "ApiError"
                ? `${error.message} Inténtalo de nuevo.`
                : "No se pudo conectar con la API para eliminar el curso. Inténtalo de nuevo.";
            courseError.hidden = false;
        }
    }

    function openEditDialog(course) {
        editingCourseId = course.id;
        editForm.elements.codigo.value = course.codigo;
        editForm.elements.nombre.value = course.nombre;
        editForm.elements.area.value = course.area;
        editForm.elements.horas.value = course.horas;
        editForm.elements.cupoMaximo.value = course.cupoMaximo;
        editForm.elements.fechaInicio.value = course.fechaInicio;
        editError.hidden = true;
        editDialog.showModal();
    }

    document.querySelector("#cancel-edit-course").addEventListener("click", () => editDialog.close());

    editForm.addEventListener("submit", async (event) => {
        event.preventDefault();
        editError.hidden = true;
        courseSuccess.hidden = true;
        courseError.hidden = true;
        if (!validateCourseForm(editForm, editError)) {
            return;
        }

        try {
            const response = await fetch(`/api/cursos/${editingCourseId}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(getCourseFromForm(editForm))
            });

            if (!response.ok) {
                throw createApiError(await getApiError(response, "No se pudo actualizar el curso."));
            }

            editDialog.close();
            courseSuccess.textContent = "Curso actualizado correctamente.";
            courseSuccess.hidden = false;
            await loadCourses();
        } catch (error) {
            editError.textContent = error.name === "ApiError"
                ? `${error.message} Revisa los datos e inténtalo de nuevo.`
                : "No se pudo conectar con la API para actualizar el curso. Inténtalo de nuevo.";
            editError.hidden = false;
        }
    });

    courseForm.addEventListener("submit", async (event) => {
        event.preventDefault();
        courseSuccess.hidden = true;
        courseError.hidden = true;
        if (!validateCourseForm(courseForm, courseError)) {
            return;
        }

        try {
            const response = await fetch("/api/cursos", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(getCourseFromForm(courseForm))
            });

            if (!response.ok) {
                throw createApiError(await getApiError(response, "No se pudo crear el curso."));
            }

            courseForm.reset();
            courseSuccess.textContent = "Curso creado correctamente.";
            courseSuccess.hidden = false;
            await loadCourses();
        } catch (error) {
            courseError.textContent = error.name === "ApiError"
                ? `${error.message} Revisa los datos e inténtalo de nuevo.`
                : "No se pudo conectar con la API para crear el curso. Inténtalo de nuevo.";
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