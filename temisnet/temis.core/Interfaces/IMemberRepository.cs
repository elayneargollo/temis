using System.Collections.Generic;
using temis.Core.Models;

namespace temis.Core.Interfaces
{
    public interface IMemberRepository
    {
         Member CreateMember(Member member);
         Member EditMember(Member member);
         List<Member> FindAll();
         Member FindById(long id);
         IEnumerable<Member> PartialEditMember(string name);
         void EditPassword(long id, string password);
         void Delete(long Id);
         PageResponse<Member> Filter (string name, PageRequest pageRequest);
    }
}