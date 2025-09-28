using Microfinance_Loan_Management_System.BusinessLogic.Models;
using Microfinance_Loan_Management_System.DataAccess;
using Microfinance_Loan_Management_System.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microfinance_Loan_Management_System.BusinessLogic.Services
{
    public class GroupService
    {
        private GroupRepository groupRepository;
        private MemberRepository memberRepository;

        public GroupService()
        {
            groupRepository = new GroupRepository();
            memberRepository = new MemberRepository();
        }

        public List<Group> GetAllGroups()
        {
            return groupRepository.GetAll();
        }

        public Group GetGroupById(int groupID)
        {
            return groupRepository.GetById(groupID);
        }

        public bool CreateGroup(Group group)
        {
            try
            {
                if (!ValidateGroup(group))
                {
                    return false;
                }

                group.CreatedDate = DateTime.Now;
                group.IsActive = true;

                return groupRepository.Insert(group);
            }
            catch (Exception ex)
            {
                throw new Exception("Group creation failed: " + ex.Message);
            }
        }

        public bool UpdateGroup(Group group)
        {
            try
            {
                if (!ValidateGroup(group))
                {
                    return false;
                }

                Group existingGroup = groupRepository.GetById(group.GroupID);
                if (existingGroup != null && group.MaxMembers < existingGroup.GetCurrentMemberCount())
                {
                    throw new Exception("Cannot reduce maximum members below current member count");
                }

                return groupRepository.Update(group);
            }
            catch (Exception ex)
            {
                throw new Exception("Group update failed: " + ex.Message);
            }
        }

        public bool DeactivateGroup(int groupID)
        {
            try
            {
                Group group = groupRepository.GetById(groupID);
                if (group == null) return false;

                if (group.GetCurrentMemberCount() > 0)
                {
                    throw new Exception("Cannot deactivate group with active members. Please remove all members first.");
                }

                return groupRepository.Delete(groupID);
            }
            catch (Exception ex)
            {
                throw new Exception("Group deactivation failed: " + ex.Message);
            }
        }

        public bool AddMemberToGroup(int memberID, int groupID)
        {
            try
            {
                Member member = memberRepository.GetById(memberID);
                if (member == null)
                {
                    throw new Exception("Member not found");
                }

                if (member.GroupID.HasValue)
                {
                    throw new Exception("Member is already assigned to a group");
                }

                Group group = groupRepository.GetById(groupID);
                if (group == null)
                {
                    throw new Exception("Group not found");
                }

                if (!group.IsActive)
                {
                    throw new Exception("Cannot add member to inactive group");
                }

                return groupRepository.AddMemberToGroup(memberID, groupID);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add member to group: " + ex.Message);
            }
        }

        public bool RemoveMemberFromGroup(int memberID)
        {
            try
            {
                LoanRepository loanRepo = new LoanRepository();
                List<Loan> memberLoans = loanRepo.GetLoansByMember(memberID);

                bool hasActiveLoans = memberLoans.Any(l => l.Status == "Active" || l.Status == "Approved");
                if (hasActiveLoans)
                {
                    throw new Exception("Cannot remove member with active loans from group");
                }

                return groupRepository.RemoveMemberFromGroup(memberID);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to remove member from group: " + ex.Message);
            }
        }

        public List<Group> GetAvailableGroups()
        {
            return groupRepository.GetAvailableGroups();
        }

        public List<Member> GetGroupMembers(int groupID)
        {
            return groupRepository.GetGroupMembers(groupID);
        }

        public DataTable GetGroupStatistics()
        {
            return groupRepository.GetGroupStatistics();
        }

        public bool TransferMember(int memberID, int fromGroupID, int toGroupID)
        {
            try
            {
                Group targetGroup = groupRepository.GetById(toGroupID);
                if (targetGroup == null || !targetGroup.IsActive)
                {
                    throw new Exception("Invalid target group");
                }

                if (targetGroup.GetCurrentMemberCount() >= targetGroup.MaxMembers)
                {
                    throw new Exception("Target group has reached maximum capacity");
                }

                return groupRepository.AddMemberToGroup(memberID, toGroupID);
            }
            catch (Exception ex)
            {
                throw new Exception("Member transfer failed: " + ex.Message);
            }
        }

        public List<Group> SearchGroups(string searchTerm)
        {
            List<Group> allGroups = GetAllGroups();
            return allGroups.Where(g =>
                g.GroupName.ToLower().Contains(searchTerm.ToLower()) ||
                (g.GroupLeader != null && g.GroupLeader.ToLower().Contains(searchTerm.ToLower()))
            ).ToList();
        }

        public bool CanDeleteGroup(int groupID)
        {
            Group group = groupRepository.GetById(groupID);
            if (group == null) return false;

            if (group.GetCurrentMemberCount() > 0) return false;

            string query = "SELECT COUNT(*) FROM Loans WHERE GroupID = @GroupID";
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupID", groupID);
                conn.Open();
                int loanCount = (int)cmd.ExecuteScalar();
                return loanCount == 0;
            }
        }

        private bool ValidateGroup(Group group)
        {
            if (string.IsNullOrWhiteSpace(group.GroupName))
            {
                throw new Exception("Group name is required");
            }

            if (group.GroupName.Length > 100)
            {
                throw new Exception("Group name cannot exceed 100 characters");
            }

            if (group.MaxMembers <= 0 || group.MaxMembers > 50)
            {
                throw new Exception("Maximum members must be between 1 and 50");
            }

            List<Group> existingGroups = groupRepository.GetAll();
            bool isDuplicate = existingGroups.Any(g =>
                g.GroupName.Equals(group.GroupName, StringComparison.OrdinalIgnoreCase) &&
                g.GroupID != group.GroupID);

            if (isDuplicate)
            {
                throw new Exception("A group with this name already exists");
            }

            return true;
        }
    }
}