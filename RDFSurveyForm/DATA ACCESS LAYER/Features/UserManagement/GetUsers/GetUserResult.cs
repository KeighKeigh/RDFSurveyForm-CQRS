namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers
{
    public partial class GetUser
    {
        public record class GetUserResult
        {
            public int Id { get; set; }
            public string Fullname { get; set; }
            public string Username {  get; set; }
            public int? RoleId { get; set; }
            public string Role_Name { get; set; }

            public int? GroupsId {  get; set; }
            public string Group_Name { get; set; }

            public int? DepartmentId { get; set; }
            public string Department_Name { get;set; }
            public string Created_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Updated_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool Is_Archive { get; set; }
        }
    }
}
