﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.DataAccessLayer.IR_Setup.Interface;
using RDFSurveyForm.Dto.ModelDto.UserDto;
using RDFSurveyForm.Dto.SetupDto.GroupDto;
using RDFSurveyForm.Dto.SetupDto.GroupSurveyDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Model.Setup;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Emit;


namespace RDFSurveyForm.DataAccessLayer.IR_Setup.Repository
{
    public class GroupSurveyRepository : IGroupSurveyRepository
    {
        private readonly StoreContext _context;

        public GroupSurveyRepository(StoreContext context)
        {
            _context = context;
        }


        public async Task<bool> AddSurvey(AddGroupSurveyDto survey)
        {          
                var newGenerator = new SurveyGenerator { };
            await _context.SurveyGenerator.AddAsync(newGenerator);
            await _context.SaveChangesAsync();


            var addGroupId = new GroupSurvey
            {
                GroupsId = survey.GroupsId,
                CreatedAt = DateTime.Now,
                CreatedBy = survey.CreatedBy,
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

            foreach (var item in survey.UpdateSurveyScores)
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
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> GroupIdDoesnotExist(int? Id)
        {
            var groupExist = await _context.Groups.AnyAsync(x => x.Id == Id);
            if(groupExist == true)
            {
                return true;
            }
            return false;
        }

        public async Task<PagedList<GetGroupSurveyDto>> GroupSurveyPagination(UserParams userParams, bool? status, string search)
        {
            //var departmentId = _context.SurveyScores.
            var totalScore = _context.SurveyScores
            .GroupBy(x => new
            {
                x.SurveyGeneratorId,
                x.Id,    
                x.Score,
                x.Limit,
                //Ids = Ids, 
            }).Select(x => new ScoreDto
            {
                SurveyGeneratorId = x.Key.SurveyGeneratorId,
                Id = x.Key.Id,
                Score = x.Key.Score / x.Key.Limit,
                Limit = x.Key.Limit,
                ActualScore = x.Key.Score
            });
             

            var categoryPercentage = _context.SurveyScores
              .GroupJoin(totalScore, total => total.Id, percentage => percentage.Id, (total, percentage) => new { total, percentage })
              .SelectMany(x => x.percentage.DefaultIfEmpty(), (x, percentage) => new { x.total, percentage })
              .GroupBy(x => new
              {
                  x.total.SurveyGeneratorId,
                  x.total.Id,
                  x.total.CategoryPercentage,
              }).Select(x => new ScoreDto
              {
                  SurveyGeneratorId = x.Key.SurveyGeneratorId,
                  Id = x.Key.Id,
                  Score = x.Sum(x => x.percentage.Score) * x.Key.CategoryPercentage,
                  CategoryPercentage = x.Key.CategoryPercentage,
              });


            var num = 0;
            int? userss = 0;
            var counts = false;
            var number = await _context.GroupSurvey.Where(x => x.GroupsId != 0 && x.IsActive == true).ToListAsync();
            foreach (var group in number)
            {
                
                
                if (group.GroupsId != null  && counts == false) 
                {
                    userss = group.GroupsId;
                    counts = true;
                }

                if(userss == group.GroupsId)
                {
                    num++;
                }

            }
            
            var users = _context.GroupSurvey
                    .GroupJoin( categoryPercentage, score => score.SurveyGeneratorId, percentage => percentage.SurveyGeneratorId, (score, percentage) => new { score, percentage })
                    .SelectMany(x => x.percentage.DefaultIfEmpty(), (x, percentage) => new { x.score, percentage })
                    .GroupBy(x => x.score.GroupsId )
                    .Select(x => new GetGroupSurveyDto
                    {
                        SurveyGeneratorId = x.Key,
                        BranchName = x.First().score.Groups.Branch.BranchName,
                        IsActive = x.First().score.IsActive,
                        CreatedBy = x.First().score.CreatedBy,
                        CreatedAt = x.First().score.CreatedAt,
                        GroupName = x.First().score.Groups.GroupName,
                        IsTransacted = x.First().score.IsTransacted,
                        FinalScore = (x.Sum(x => x.percentage.Score) * 100) / num
                    });


            //if (Ids != null)
            //{


                
            //    //var groupid = await _context.GroupSurvey.Where(x => x.GroupsId  == users.);
            //    int count =  userss.Count();
            //    foreach (var items in userss)
            //    { 


            //        items.FinalScore = items.FinalScore ;
            //        users =  users.Where(x => x.GroupName == items.GroupName);
                    
            //    }


            //}


            if (status != null)
                {
                    users = users.Where(x => x.IsActive == status);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    users = users.Where(x => x.SurveyGeneratorId.ToString().Contains(search)
                    || Convert.ToString(x.BranchName).ToLower().Contains(search.Trim().ToLower())
                    || Convert.ToString(x.GroupName).ToLower().Contains(search.Trim().ToLower()));
                }

                users = users.OrderByDescending(x => x.FinalScore);

                return await PagedList<GetGroupSurveyDto>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
            
             }

        public async Task<IReadOnlyList<ViewSurveyDto>> ViewSurvey(int? id)
        {
            var results = _context.SurveyScores.Where(x => x.SurveyGeneratorId == id)
                .GroupBy(x => x.SurveyGeneratorId).Select(x => new ViewSurveyDto
                {
                    SurveyGeneratorId = x.Key,
                    Categories = x.Select(x => new ViewSurveyDto.Category
                    {
                        Id = x.Id,
                        CategoryName = x.CategoryName,
                        CategoryPercentage = x.CategoryPercentage * 100,
                        Score = x.Score,
                        Limit = x.Limit,
                        SurveyPercentage =  (x.Score / x.Limit) * x.CategoryPercentage,

                    }).ToList()

                }).ToListAsync();

            return await results;
        }

        public async Task<bool> ScoreLimit(AddGroupSurveyDto limit)
        {
            
            foreach (var items in limit.UpdateSurveyScores)
            {
                //var lim = await _context.SurveyScores.FirstOrDefaultAsync(x => x.Score == items.Score);
                /*var limitless = await _context.SurveyScores.FirstOrDefaultAsync(x => x.Score == items.Score)*/;
                var limits = await _context.Category.FirstOrDefaultAsync(x => x.Limit >= items.Score);
                                
                if( limits == null || items.Score < 0)
                {
                    return false;
                }
                
                //if (limits > lim.Limit || limits < 0)
                //{
                //    return false;
                //}
                
            }
            return true;
        }


        public async Task<bool> UpdateScore(UpdateSurveyScoreDto score)
        {

            var updateScore = await _context.GroupSurvey.Where(x => x.GroupsId == score.GroupsId
            && x.IsActive == true).ToListAsync();
            foreach (var items in updateScore)
            {



                items.UpdatedBy = score.UpdatedBy;
                items.UpdatedAt = score.UpdatedAt;
                items.IsTransacted = true;
 
            }
            await _context.SaveChangesAsync();

            return true;

        }
        public async Task<bool> SetIsActive(int Id)
        {
            var setIsactive = await _context.GroupSurvey.FirstOrDefaultAsync(x => x.SurveyGeneratorId == Id);
            var setIsactivee = await _context.SurveyGenerator.FirstOrDefaultAsync(x => x.Id == Id);
            var setIsactiveee = await _context.SurveyScores.Where(x => x.SurveyGeneratorId == Id).ToListAsync();
            if (setIsactive != null)
            {
                setIsactive.IsActive = !setIsactive.IsActive;
                setIsactivee.IsActive = !setIsactivee.IsActive;
                foreach(var item in setIsactiveee) 
                {
                    item.IsActive = !item.IsActive;

                }
                
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
