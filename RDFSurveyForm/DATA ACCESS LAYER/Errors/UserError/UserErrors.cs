﻿using RDFSurveyForm.Common;

namespace RDFSurveyForm.Handlers.Errors.UserError
{
    public class UserErrors
    {
        public static Error IdDoesNotExist() => new Error("IdDoesNotExist",
            "User ID does not exist.");

        public static Error WrongPassword() => new Error("User.WrongPassword",
            "Wrong Password.");

        public static Error PassSameAsOld() => new Error("User.PassSameAsOld",
            "Password is the same as old Password.");

        public static Error PassConfirmationError() => new Error("User.PassConfirmationError",
            "Password Confirmation error.");

        public static Error UserRoleNotExist() =>
        new("User.UserRoleNotExist", "User role not exist!");

        public static Error EmptyFullName() => new Error("User.EmptyFullName",
            "Input full name.");

        public static Error EmptyUserName() => new Error("User.EmptyUserName",
            "Input username.");

        public static Error NameExist() => new Error("User.NameExist",
            "Name already exists.");

        public static Error UserNameExist() => new Error("User.USerNameExist",
            "Userame already exists.");


        //Role Errors
        public static Error RoleNameExist() => new Error("Role.RoleNameExist",
            "Role already exists.");

        public static Error PermissionTagged() => new Error("Role.PermissionTagged",
            "Tagged a Permission.");

        public static Error PermissionUntagged() => new Error("Role.PermissionUntagged",
            "Untagged a Permission.");

        //Department Errors
        public static Error DepartmentExist() => new Error("Department.DepartmentExist",
            "Department Name already exist.");

        //Branch Errors
        public static Error BranchExist() => new Error("Branch.BranchExist",
            "Branch Name already exist.");

        public static Error BranchCodeExist() => new Error("Branch.BranchCodeExist",
            "Branch Code already exist.");

        //Category Errors
        public static Error PercentageExceed() => new Error("Category.PercentageExceed",
            "% exceeded 100%");
        public static Error CategoryExist() => new Error("Category.BranchCodeExist",
            "Category Already Exist!");

        //Group Errors
        public static Error GroupExist() => new Error("Group.GroupExist",
            "Group Name Already Exist!");

        //GroupSurvey Errors
        public static Error ScoreExceed() => new Error("GroupSurvey.ScoreExceed",
            "Follow the Limit!");
        public static Error NoGroupId() => new Error("GroupSurvey.NoGroupId",
            "Group Id does not exist!");
        public static Error GroupSurveyError() => new Error("GroupSurvey.GroupSurveyError",
            "Error!");
    }
}
