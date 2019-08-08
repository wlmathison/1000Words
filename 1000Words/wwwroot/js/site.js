// Create variable for search input on navbar
const navSearchBar = document.getElementById("navSearchInput");

// Create variable for select group attached to search input
const inputGroupSelect = document.getElementById("inputGroupSelect");

inputGroupSelect.addEventListener("click", selectSearch);

function selectSearch() {
    switch (inputGroupSelect.value) {

        // If user selects search by keywords
        case "1":
            navSearchBar.type = "text";
            navSearchBar.value = "";
            navSearchBar.placeholder = "Enter keywords";
            break;

        // If user selects search by date
        case "2":
            navSearchBar.type = "date";
            navSearchBar.format = "mm/dd/yyyy";
            break;

        // If user selects search by month
        case "3":
            navSearchBar.type = "month";
            break;

        // If user selects search by year
        case "4":
            navSearchBar.type = "number";
            navSearchBar.placeholder = "";
            navSearchBar.min = 1900;
            navSearchBar.max = new Date().getFullYear();
            break;
    }
}