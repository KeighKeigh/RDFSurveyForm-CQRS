using RDFSurveyForm.Common;

namespace RDFSurveyForm.Handlers.Errors.UserError
{
    public class UserErrors
    {
        public static Error IdDoesNotExist() => new Error("User.IdDoesNotExist",
            "User ID does not exist");

        public static Error WrongPassword() => new Error("User.WrongPassword",
            "Wrong Password");

        public static Error PassSameAsOld() => new Error("User.PassSameAsOld",
            "Password is the same as old Password");

        public static Error PassConfirmationError() => new Error("User.PassConfirmationError",
            "Password Confirmation error");

        public static Error UserRoleNotExist() =>
        new("User.UserRoleNotExist", "User role not exist!");

        public static Error EmptyFullName() => new Error("User.EmptyFullName",
            "Input full name.");

        public static Error EmptyUserName() => new Error("User.EmptyUserName",
            "Input username.");

        public static Error NameExist() => new Error("User.NameExist",
            "Name already exists");

        public static Error UserNameExist() => new Error("User.USerNameExist",
            "Userame already exists");

        public static Error RoleNameExist() => new Error("User.RoleNameExist",
            "Role already exists");

    }
}
