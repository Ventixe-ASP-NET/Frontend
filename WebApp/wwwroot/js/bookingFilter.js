document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('bookingSearch');
    const categorySelect = document.getElementById('categoryFilter');

    function filterBookings() {
        const search = searchInput.value.toLowerCase();
        const selectedCategory = categorySelect.value.toLowerCase();
        const rows = document.querySelectorAll('.booking-table tbody tr');

        rows.forEach(row => {
            const title = row.getAttribute('data-title') || '';
            const category = row.getAttribute('data-category') || '';

            const matchesSearch = title.includes(search);
            const matchesCategory = selectedCategory === '' || category === selectedCategory;

            row.style.display = matchesSearch && matchesCategory ? 'table-row' : 'none';
        });
    }

    if (searchInput) searchInput.addEventListener('input', filterBookings);
    if (categorySelect) categorySelect.addEventListener('change', filterBookings);

    filterBookings(); // Init vid load
});