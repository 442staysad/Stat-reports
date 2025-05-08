document.addEventListener('DOMContentLoaded', function () {
    const reportTypeSelect = document.getElementById('reportType');
    const templateSelect = document.getElementById('templateId');
    const allTemplates = Array.from(templateSelect.options).filter(opt => opt.value !== "");

    function filterTemplatesByType(type) {
        templateSelect.innerHTML = '<option value="">Все</option>';
        const filtered = allTemplates.filter(opt => opt.dataset.type === type || type === "");
        filtered.forEach(opt => templateSelect.appendChild(opt));
    }

    reportTypeSelect.addEventListener('change', () => {
        filterTemplatesByType(reportTypeSelect.value);
    });

    // Инициализировать при загрузке (на случай возврата после фильтрации)
    filterTemplatesByType(reportTypeSelect.value);

    // Сброс фильтров
    document.getElementById('resetFilters').addEventListener('click', () => {
        const form = document.querySelector('form[method="get"]');

        // Сбросить текстовые и date поля
        form.querySelectorAll('input[type="text"], input[type="date"]').forEach(input => input.value = '');

        // Сбросить select-поля
        form.querySelectorAll('select').forEach(select => {
            if (!select.disabled) {
                select.selectedIndex = 0;
            }
        });

        // Обновить шаблоны
        filterTemplatesByType("");

        // Отправить форму
        form.submit();
    });

    document.addEventListener('DOMContentLoaded', function () {
        const tableBody = document.querySelector('table tbody');
        const rows = Array.from(tableBody.querySelectorAll('tr')).filter(r => !r.classList.contains('no-data'));
        const rowsPerPage = 10;
        const paginationContainer = document.getElementById('pagination');
        let currentPage = 1;

        function renderTablePage(page) {
            const start = (page - 1) * rowsPerPage;
            const end = start + rowsPerPage;

            rows.forEach((row, index) => {
                row.style.display = index >= start && index < end ? '' : 'none';
            });
        }

        function renderPagination() {
            paginationContainer.innerHTML = '';
            const pageCount = Math.ceil(rows.length / rowsPerPage);

            if (pageCount <= 1) return;

            for (let i = 1; i <= pageCount; i++) {
                const btn = document.createElement('button');
                btn.textContent = i;
                btn.className = 'btn btn-sm mx-1 ' + (i === currentPage ? 'btn-primary' : 'btn-outline-primary');
                btn.addEventListener('click', () => {
                    currentPage = i;
                    renderTablePage(currentPage);
                    renderPagination();
                });
                paginationContainer.appendChild(btn);
            }
        }

        if (rows.length > 0) {
            renderTablePage(currentPage);
            renderPagination();
        }
    });