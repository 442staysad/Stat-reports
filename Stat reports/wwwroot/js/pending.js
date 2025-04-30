document.addEventListener('DOMContentLoaded', () => {
    const typeTabs = document.querySelectorAll('#reportTypeTabs .nav-link');
    const branchTabs = document.querySelectorAll('#branchTabs .nav-link');

    // Функция фильтрации строк по reportType для всех табов филиалов
    function filterAllBranches(reportType) {
        document.querySelectorAll('.tab-pane').forEach(pane => {
            pane.querySelectorAll('tbody tr').forEach(row => {
                const rowType = row.getAttribute('data-report-type');
                row.style.display = (!reportType || rowType === reportType) ? '' : 'none';
            });
        });
    }

    // хук на клики табов типов
    typeTabs.forEach(tab => {
        tab.addEventListener('click', () => {
            typeTabs.forEach(t => t.classList.remove('active'));
            tab.classList.add('active');
            const rt = tab.getAttribute('data-report-type');
            filterAllBranches(rt);
        });
    });

    // при переключении филиала — повторно применяем текущий фильтр
    branchTabs.forEach(tab => {
        tab.addEventListener('shown.bs.tab', () => {
            const activeTypeTab = document.querySelector('#reportTypeTabs .nav-link.active');
            const rt = activeTypeTab.getAttribute('data-report-type');
            filterAllBranches(rt);
        });
    });

    // инициализация: все, без фильтра
    filterAllBranches('');
});