// Course cards are filtered locally so searching never resets the booking form.
document.querySelectorAll('[data-course-picker]').forEach(picker => {
    const search = picker.querySelector('[data-course-search]');
    const cards = Array.from(picker.querySelectorAll('[data-course-card]'));
    const count = picker.querySelector('[data-course-count]');
    const empty = picker.querySelector('[data-course-empty]');
    const loadMore = picker.querySelector('[data-course-load-more]');
    const pageSize = 4;
    let visibleCount = pageSize;

    const render = () => {
        const term = search.value.trim().toLocaleLowerCase();
        let matches = 0;

        cards.forEach((card, index) => {
            const matchesSearch = card.dataset.courseSearchText.toLocaleLowerCase().includes(term);
            const selected = card.querySelector('input').checked;
            const inCurrentPage = index < visibleCount;
            card.classList.toggle('d-none', !selected && (term ? !matchesSearch : !inCurrentPage));
            if (matchesSearch) matches++;
        });

        const hasSelection = cards.some(card => card.querySelector('input').checked);
        count.textContent = matches === 0
            ? (hasSelection ? 'No matches; selected course shown' : 'No matching courses')
            : `${matches} ${matches === 1 ? 'course' : 'courses'} found`;
        if (empty) empty.hidden = matches > 0 || hasSelection;
        if (loadMore) {
            loadMore.hidden = term.length > 0 || visibleCount >= cards.length;
            if (!loadMore.hidden) {
                const remaining = Math.min(pageSize, cards.length - visibleCount);
                loadMore.textContent = `Load ${remaining} more ${remaining === 1 ? 'course' : 'courses'}`;
            }
        }
    };

    if (loadMore) {
        loadMore.addEventListener('click', () => {
            visibleCount = Math.min(visibleCount + pageSize, cards.length);
            render();
        });
    }

    search.addEventListener('input', render);
    search.addEventListener('keydown', event => {
        if (event.key === 'Enter' || event.key === 'ArrowDown') {
            event.preventDefault();
            cards.find(card => !card.hidden)?.querySelector('input').focus();
        }
    });
    cards.forEach(card => card.querySelector('input').addEventListener('change', render));
    render();
});

document.querySelectorAll('[data-load-more]').forEach(list => {
    const items = Array.from(list.querySelectorAll('[data-load-more-item]'));
    const pageSize = Number.parseInt(list.dataset.loadMoreSize, 10);
    const controls = list.parentElement.querySelector('[data-load-more-controls]');
    const button = controls?.querySelector('[data-load-more-button]');
    const count = controls?.querySelector('[data-load-more-count]');
    const label = list.dataset.loadMoreLabel || 'items';
    let visibleCount = pageSize;

    if (!button || !Number.isInteger(pageSize) || pageSize < 1) return;

    const render = () => {
        items.forEach((item, index) => item.classList.toggle('d-none', index >= visibleCount));
        if (count) count.textContent = `Showing ${Math.min(visibleCount, items.length)} of ${items.length} ${label}`;
        if (controls) controls.hidden = visibleCount >= items.length;
    };

    button.addEventListener('click', () => {
        visibleCount = Math.min(visibleCount + pageSize, items.length);
        render();
    });

    render();
});
