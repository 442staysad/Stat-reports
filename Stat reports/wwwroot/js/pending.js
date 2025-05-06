document.addEventListener('DOMContentLoaded', () => {
    const typeTabs = document.querySelectorAll('#reportTypeTabs .nav-link');
    const branchTabs = document.querySelectorAll('#branchTabs .nav-link');
    let currentPage = 1;
    const rowsPerPage = 10;  // количество строк на странице

    // Функция фильтрации и пагинации строк по reportType для всех табов филиалов
    function filterAndPaginate(reportType) {
        document.querySelectorAll('.tab-pane').forEach(pane => {
            pane.querySelectorAll('tbody tr').forEach((row, index) => {
                const rowType = row.getAttribute('data-report-type');
                const isVisible = !reportType || rowType === reportType;
                row.style.display = isVisible ? '' : 'none';

                // Пагинация: показывать только для текущей страницы
                if (isVisible) {
                    const rowIndex = index + 1;  // Индекс строки, начиная с 1
                    const startIndex = (currentPage - 1) * rowsPerPage + 1;
                    const endIndex = currentPage * rowsPerPage;
                    row.style.display = (rowIndex >= startIndex && rowIndex <= endIndex) ? '' : 'none';
                }
            });
        });
    }

    // хук на клики табов типов
    typeTabs.forEach(tab => {
        tab.addEventListener('click', () => {
            typeTabs.forEach(t => t.classList.remove('active'));
            tab.classList.add('active');
            const rt = tab.getAttribute('data-report-type');
            filterAndPaginate(rt);
        });
    });

    // при переключении филиала — повторно применяем текущий фильтр
    branchTabs.forEach(tab => {
        tab.addEventListener('shown.bs.tab', () => {
            const activeTypeTab = document.querySelector('#reportTypeTabs .nav-link.active');
            const rt = activeTypeTab.getAttribute('data-report-type');
            filterAndPaginate(rt);
        });
    });

    // Обработчики для кнопок пагинации
    document.querySelectorAll('.prev-page').forEach(button => {
        button.addEventListener('click', () => {
            if (currentPage > 1) {
                currentPage--;
                const activeTypeTab = document.querySelector('#reportTypeTabs .nav-link.active');
                const rt = activeTypeTab.getAttribute('data-report-type');
                filterAndPaginate(rt);
            }
        });
    });

    document.querySelectorAll('.next-page').forEach(button => {
        button.addEventListener('click', () => {
            const totalRows = document.querySelectorAll('.tab-pane .table tbody tr').length;
            const totalPages = Math.ceil(totalRows / rowsPerPage);
            if (currentPage < totalPages) {
                currentPage++;
                const activeTypeTab = document.querySelector('#reportTypeTabs .nav-link.active');
                const rt = activeTypeTab.getAttribute('data-report-type');
                filterAndPaginate(rt);
            }
        });
    });

    // инициализация: все, без фильтра
    filterAndPaginate('');
});