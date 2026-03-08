export const passwordRegex =
    /^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*(),.?":{}|<>]).{8,}$/;

export function getPasswordStrength(pass) {
    let score = 0;

    if (pass.length >= 8) score++;
    if (/[0-9]/.test(pass)) score++;
    if (/[!@#$%^&*(),.?":{}|<>]/.test(pass)) score++;
    if (/[A-Z]/.test(pass)) score++;

    if (score <= 1) return {label: "Weak", class: "weak"};
    if (score === 2 || score === 3) return {label: "Medium", class: "medium"};
    return {label: "Strong", class: "strong"};
}

export function getPasswordRules(pass) {
    return {
        length: pass.length >= 8,
        number: /\d/.test(pass),
        special: /[!@#$%^&*(),.?":{}|<>]/.test(pass),
        capital: /[A-Z]/.test(pass)
    };
}

export function isPasswordValid(pass) {
    return passwordRegex.test(pass);
}