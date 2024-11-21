using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model.Setup;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupSurveyManagement.AddGroupSurvey
{
    public class AddGroupSurveyHandler
    {
        public class AddGroupSurveyCommand : IRequest<Result>
        {
            public int? GroupsId { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; }

            public int? SurveyGeneratorId { get; set; }


            public List<UpdateSurveyScore> UpdateSurveyScores { get; set; }
            public class UpdateSurveyScore
            {
                public decimal Score { get; set; }
            }
        }

        public class Handler : IRequestHandler<AddGroupSurveyCommand, Result>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {

                _context = context;

            }
            async public Task<Result> Handle(AddGroupSurveyCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CreateGroupSurvey(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            private async Task<Result> Validator(AddGroupSurveyCommand command, CancellationToken cancellationToken)
            {
                foreach (var items in command.UpdateSurveyScores)
                {                   
                    var limits = await _context.Category.FirstOrDefaultAsync(x => x.Limit >= items.Score);

                    if (limits == null || items.Score < 0)
                    {
                        return Result.Failure(UserErrors.ScoreExceed());
                    }
                }

                bool groupExist = await _context.Groups.AnyAsync(x => x.Id == command.GroupsId);
                if (!groupExist)
                {
                    return Result.Failure(UserErrors.NoGroupId());
                }

                return null;

            }

            private async Task CreateGroupSurvey(AddGroupSurveyCommand command, CancellationToken cancellationToken)
            {
                var newGenerator = new SurveyGenerator { };
                await _context.SurveyGenerator.AddAsync(newGenerator);
                await _context.SaveChangesAsync();


                var addGroupId = new GroupSurvey
                {
                    GroupsId = command.GroupsId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = command.CreatedBy,
                    SurveyGeneratorId = newGenerator.Id,

                };
                await _context.GroupSurvey.AddAsync(addGroupId);


                var categoryList = await _context.Category.Where(x => x.IsActive).ToListAsync();

                foreach (var itemss in categoryList)
                {




                    var addSurveyScore = new SurveyScore
                    {
                        CategoryName = itemss.CategoryName,
                        CategoryPercentage = itemss.CategoryPercentage,
                        Limit = itemss.Limit,
                        SurveyGeneratorId = newGenerator.Id,
                        CreatedBy = itemss.CreatedBy,

                    };
                    await _context.SurveyScores.AddAsync(addSurveyScore);
                    await _context.SaveChangesAsync();



                }

                foreach (var item in command.UpdateSurveyScores)
                {


                    var scorelist = await _context.SurveyScores.Where(x => x.SurveyGeneratorId == newGenerator.Id).ToListAsync();
                    foreach (var scorel in scorelist)
                    {

                        var scores = await _context.SurveyScores.FirstOrDefaultAsync(x => x.Id == scorel.Id);

                        if (scores.Score == 0)
                        {
                            if (scores != null)
                            {
                                scores.Score = item.Score;

                                break;
                            }

                        }
                    }
                }               
            }
        }
    }
}
