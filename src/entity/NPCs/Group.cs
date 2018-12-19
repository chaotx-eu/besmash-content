using System.Collections.Generic;

namespace BesmashContent
{
    public class Group
    {
        public List<Enemy> member = new List<Enemy>();

        public void addMember(Enemy newMember){
            member.Add(newMember);
        }
        public List<Enemy> getMember(){
            return member;
        }
    }
}