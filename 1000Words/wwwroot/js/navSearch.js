const dropdownItems = document.getElementsByName("search-dropdown-item");
const navSearchBar = document.getElementById("navSearchInput");
const dropdownTitle = document.getElementById("dropdown-title");

dropdownItems.forEach(d => d.addEventListener("click", () => selectSearch(d)));
function selectSearch(d) {
    switch (d.text) {
        case "Keywords":
            dropdownTitle.textContent = "Search by keywords";
            navSearchBar.type = "text";
            navSearchBar.value = "";
            navSearchBar.placeholder = "Enter keywords";
            navSearchBar.name = "searchString";
            //navSearchBar.value = "@ViewData['currentFilter']";
            break;

        case "Day":
            dropdownTitle.textContent = "Search by day";
            navSearchBar.type = "date";
            navSearchBar.name = "searchDay";
            //navSearchBar.value = "@ViewData['currentFilter']";
            break;

        case "Month":
            dropdownTitle.textContent = "Search by month";
            navSearchBar.type = "month";
            navSearchBar.name = "searchMonth";
            //navSearchBar.value = "@ViewData['currentFilter']";
            break;

        case "Year":
            dropdownTitle.textContent = "Search by year";
            navSearchBar.type = "number";
            navSearchBar.name = "searchYear";
            navSearchBar.min = 1900;
            navSearchBar.max = new Date().getFullYear();
            //navSearchBar.value = "@ViewData['currentFilter']";
            break;
    }
}