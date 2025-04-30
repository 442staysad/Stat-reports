
        // Инициализация всплывающих подсказок
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
        });

        // Автоматический размер textarea для комментария
        document.querySelectorAll('textarea').forEach(el => {
        el.style.height = el.setAttribute('style', 'height: ' + el.scrollHeight + 'px');
            el.addEventListener('input', e => {
        el.style.height = 'auto';
    el.style.height = (el.scrollHeight) + 'px';
            });
        });

        // Навигация по листам
        document.querySelectorAll('.nav-arrow').forEach(arrow => {
        arrow.addEventListener('click', function () {
            const sheetContainer = this.closest('.sheet-container');
            const currentIndex = parseInt(sheetContainer.dataset.sheetIndex);
            const allSheets = Array.from(document.querySelectorAll('.sheet-container'));

            if (this.classList.contains('prev-sheet')) {
                if (currentIndex > 0) {
                    sheetContainer.style.display = 'none';
                    allSheets[currentIndex - 1].style.display = 'block';
                }
            } else if (this.classList.contains('next-sheet')) {
                if (currentIndex < allSheets.length - 1) {
                    sheetContainer.style.display = 'none';
                    allSheets[currentIndex + 1].style.display = 'block';
                }
            }
        });
        });

        // Горизонтальная прокрутка таблиц
        document.querySelectorAll('.scrollable-table').forEach(tableContainer => {
            const scrollLeftBtn = tableContainer.querySelector('.scroll-left');
    const scrollRightBtn = tableContainer.querySelector('.scroll-right');
    const table = tableContainer.querySelector('table');

            scrollLeftBtn.addEventListener('click', () => {
        tableContainer.scrollBy({ left: -200, behavior: 'smooth' });
            });

            scrollRightBtn.addEventListener('click', () => {
        tableContainer.scrollBy({ left: 200, behavior: 'smooth' });
            });

            // Показываем/скрываем кнопки в зависимости от позиции прокрутки
            tableContainer.addEventListener('scroll', () => {
                const showLeft = tableContainer.scrollLeft > 0;
    const showRight = tableContainer.scrollLeft < (table.scrollWidth - tableContainer.clientWidth);

    scrollLeftBtn.style.display = showLeft ? 'flex' : 'none';
    scrollRightBtn.style.display = showRight ? 'flex' : 'none';
            });

    // Инициализация видимости кнопок
    tableContainer.dispatchEvent(new Event('scroll'));
        });

    // Обработка клавиш клавиатуры
    document.addEventListener('keydown', function(e) {
            const activeSheet = document.querySelector('.sheet-container[style*="display: block"]') ||
    document.querySelector('.sheet-container');

    if (!activeSheet) return;

    const currentIndex = parseInt(activeSheet.dataset.sheetIndex);
    const allSheets = Array.from(document.querySelectorAll('.sheet-container'));

    if (e.key === 'ArrowLeft') {
                // Переход к предыдущему листу или прокрутка таблицы влево
                const tableContainer = activeSheet.querySelector('.scrollable-table');
                if (tableContainer.scrollLeft > 0) {
        tableContainer.scrollBy({ left: -200, behavior: 'smooth' });
                } else if (currentIndex > 0) {
        activeSheet.style.display = 'none';
    allSheets[currentIndex - 1].style.display = 'block';
                }
    e.preventDefault();
            } else if (e.key === 'ArrowRight') {
                // Переход к следующему листу или прокрутка таблицы вправо
                const tableContainer = activeSheet.querySelector('.scrollable-table');
    const table = tableContainer.querySelector('table');

    if (tableContainer.scrollLeft < (table.scrollWidth - tableContainer.clientWidth)) {
        tableContainer.scrollBy({ left: 200, behavior: 'smooth' });
                } else if (currentIndex < allSheets.length - 1) {
        activeSheet.style.display = 'none';
    allSheets[currentIndex + 1].style.display = 'block';
                }
    e.preventDefault();
            }
        });
