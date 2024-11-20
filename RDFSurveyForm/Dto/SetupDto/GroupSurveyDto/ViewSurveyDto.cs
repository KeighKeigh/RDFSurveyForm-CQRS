namespace RDFSurveyForm.Dto.SetupDto.GroupSurveyDto
{
    public class ViewSurveyDto
    {
        public int ? SurveyGeneratorId { get; set; }

        public List<Categoria> Categories { get; set; }
        public class Categoria
        {
            public int ? Id { get; set; }
            public string CategoryName { get; set; }
            public decimal Score { get; set; }
            public decimal Limit { get; set; }
            public decimal CategoryPercentage { get; set; }
            public decimal SurveyPercentage { get; set; }
        }

    }
}
