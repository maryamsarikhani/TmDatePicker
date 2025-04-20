console.log("📅 Persian Date Picker JS Loaded");

export function addEnterKeyListener(inputId, dotNetHelper) {
    const inputElement = document.getElementById(inputId);

    function handleKeyDown(event) {
        if (event.key === 'Enter') {
            const regex = /^\d{4}\/\d{2}\/\d{2}$/;
            if (!regex.test(inputElement.value) || inputElement.value.trim() === "") {
                inputElement.value = "____/__/__";
                dotNetHelper.invokeMethodAsync('ClosePicker');
            } else {
                dotNetHelper.invokeMethodAsync('ClosePicker');
            }
        }
    }

    inputElement.addEventListener('keydown', handleKeyDown);
    inputElement._enterKeyHandler = handleKeyDown;
}

export function registerClickOutside(pickerId, inputId, yearMonthSelectorId, dotNetHelper) {
    document.removeEventListener('click', window._persianDatePickerClickOutsideHandler);

    window._persianDatePickerClickOutsideHandler = function (event) {
        const calendarPicker = document.getElementById(pickerId);
        const inputElement = document.getElementById(inputId);
        const yearMonthSelector = document.getElementById(yearMonthSelectorId);

        const clickedOutside =
            calendarPicker && !calendarPicker.contains(event.target) &&
            (!inputElement || !inputElement.contains(event.target)) &&
            (!yearMonthSelector || !yearMonthSelector.contains(event.target));

        if (clickedOutside) {
            dotNetHelper.invokeMethodAsync('ClosePicker');
        }
    };

    document.addEventListener('click', window._persianDatePickerClickOutsideHandler, true);
}

export function removePersianDatePickerListeners(pickerId) {
    document.removeEventListener('click', window._persianDatePickerClickOutsideHandler);
    delete window._persianDatePickerClickOutsideHandler;

    const input = document.getElementById(`manualDateInput_${pickerId.split('_')[1]}`);
    if (input && input._enterKeyHandler) {
        input.removeEventListener('keydown', input._enterKeyHandler);
        delete input._enterKeyHandler;
    }
}

export function clearInputAndApplyMask(inputId, dotNetHelper) {
    const input = document.getElementById(inputId);
    if (!input) return;

    const formatDateInput = (raw) => {
        let digits = raw.replace(/\D/g, "").slice(0, 8);
        let formatted = "";

        if (digits.length >= 1 && digits[0] !== "1") return "____/__/__";

        if (digits.length >= 2 && digits[1] !== "3" && digits[1] !== "4") {
            digits = digits.slice(0, 1);
        }

        if (digits.length >= 5 && digits[4] !== "0" && digits[4] !== "1") {
            digits = digits.slice(4);
        }

        if (digits.length >= 6) {
            const monthFirstDigit = digits[4];
            if (!["0", "1", "2"].includes(monthFirstDigit)) {
                digits = digits.slice(5);
            }
        }

        if (digits.length >= 6) {
            const d1 = digits[4];
            const d2 = digits[5];
            if (d1 === "0" && !["1", "2", "3", "4", "5", "6", "7", "8", "9"].includes(d2)) {
                digits = digits.slice(0, 5);
            } else if (d1 === "1" && !["0", "1", "2"].includes(d2)) {
                digits = digits.slice(0, 5);
            }
        }

        if (digits.length >= 7) {
            const dayFirstDigit = digits[6];
            if (!["0", "1", "2", "3"].includes(dayFirstDigit)) {
                digits = digits.slice(6);
            }
        }

        if (digits.length >= 8) {
            const d1 = digits[6];
            const d2 = digits[7];

            if (d1 === "0" && !["1", "2", "3", "4", "5", "6", "7", "8", "9"].includes(d2)) {
                digits = digits.slice(0, 7);
            } else if ((d1 === "1" || d1 === "2") && !/^[0-9]$/.test(d2)) {
                digits = digits.slice(0, 7);
            } else if (d1 === "3" && !["0", "1"].includes(d2)) {
                digits = digits.slice(0, 7);
            }
        }

        if (digits.length >= 4) {
            formatted += digits.slice(0, 4) + "/";
        } else {
            formatted += digits;
            return formatted;
        }

        const monthDigits = digits.slice(4, 6);
        if (monthDigits.length === 1) {
            formatted += monthDigits;
            return formatted;
        }
        if (monthDigits.length === 2) {
            const month = parseInt(monthDigits, 10);
            if (month >= 1 && month <= 12) {
                formatted += monthDigits + "/";
            } else {
                return formatted.slice(0, 5);
            }
        }

        const dayDigits = digits.slice(6, 8);
        if (dayDigits.length === 1) {
            formatted += dayDigits;
            return formatted;
        }
        if (dayDigits.length === 2) {
            const day = parseInt(dayDigits, 10);
            if (day >= 1 && day <= 31) {
                formatted += dayDigits;
            } else {
                return formatted.slice(0, 7);
            }
        }

        return formatted;
    };

    input.addEventListener("input", () => {
        const newValue = formatDateInput(input.value);
        input.value = newValue;

        const regex = /^\d{4}\/\d{2}\/\d{2}$/;
        if (regex.test(newValue)) {
            const parts = newValue.split("/");
            const year = parseInt(parts[0], 10);
            const month = parseInt(parts[1], 10);
            const day = parseInt(parts[2], 10);

            if (year >= 1380 && year <= 1410) {
                dotNetHelper.invokeMethodAsync("UpdateCalendar", year, month, day);
            } else {
                input.value = "____/__/__";
            }
        }
    });

    input.addEventListener("keydown", (e) => {
        const allowedKeys = ["Backspace", "Delete", "ArrowLeft", "ArrowRight", "Tab"];
        const isNumber = /^[0-9]$/.test(e.key);
        if (!isNumber && !allowedKeys.includes(e.key)) {
            e.preventDefault();
        }
    });

    input.addEventListener("blur", () => {
        const val = input.value;
        const regex = /^\d{4}\/\d{2}\/\d{2}$/;

        if (regex.test(val)) {
            const parts = val.split("/");
            const year = parseInt(parts[0], 10);
            const month = parseInt(parts[1], 10);
            const day = parseInt(parts[2], 10);

            if (year >= 1380 && year <= 1410 && month >= 1 && month <= 12 && day >= 1 && day <= 31) {
                dotNetHelper.invokeMethodAsync("UpdateCalendar", year, month, day);
            } else {
                input.value = "____/__/__";
            }
        }
    });

    if (!/^\d{4}\/\d{2}\/\d{2}$/.test(input.value)) {
        input.value = "____/__/__";
    }
}

export function clearInput(inputId) {
    const input = document.getElementById(inputId);
    if (!input) return;

    input.value = "";

    // event تغییر مقدار را trigger می کنیم
    const event = new Event('input', { bubbles: true });
    input.dispatchEvent(event);

    // برای اطمینان، focus را به فیلد برگردانیم
    input.focus();
}
