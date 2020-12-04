using WaterMelon_API.Models;
using System.Linq;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace WaterMelon_API.Services
{
    public class InvitationService
    {
        private readonly IMongoCollection<Invitation> _invitations;
        private readonly IConfiguration _configuration;

        public InvitationService(IInvitationDatabaseSettings invSettings, IConfiguration config)
        {
            var client = new MongoClient(invSettings.ConnectionString);
            var invDatabase = client.GetDatabase(invSettings.DatabaseName);

            _configuration = config;
            _invitations = invDatabase.GetCollection<Invitation>(invSettings.InvitationsCollectionName);
        }

        public Invitation Create(Invitation invitation)
        {
            // You can only send an invite if it didn't exist before or if the guest previously refused 
            Invitation invitationLoaded = _invitations.Find<Invitation>(invitationQuery =>  invitationQuery.To.Equals(invitation.To)
                                                                && invitationQuery.EventId.Equals(invitation.EventId) 
                                                                && (invitationQuery.Status.Equals(InvitationStatus.Accepted) || 
                                                                invitation.Status.Equals(InvitationStatus.Pending))).FirstOrDefault();
            if (invitationLoaded == null)
            {
                _invitations.InsertOne(invitation);
                return invitation;
            }
            if (invitationLoaded.Status == InvitationStatus.Refused)
            {
                invitationLoaded.Status = InvitationStatus.Pending;
                _invitations.ReplaceOne(i => i.Id == invitationLoaded.Id, invitationLoaded);
                return invitationLoaded;
            }
            return null;
        }

        public List<Invitation> GetAllInvitations() 
        {
            var result = _invitations.Find(invitations => true).ToList();
            return result;
        } 

        public Invitation GetFromInvitationId(string id) {
            Invitation invitation = _invitations.Find<Invitation>(inv => inv.Id == id).FirstOrDefault();
            return invitation;
        } 

        public List<Invitation> GetFrom(string from)
        {
            List<Invitation> invList = _invitations.Find(i => i.From.Equals(from)).ToList();
            return invList;
        }
            

        public List<Invitation> GetTo(string to)
        {
            List<Invitation> invList = _invitations.Find(i => i.To.Equals(to)).ToList();
            return invList;
        }

        public Invitation UpdateInvitation(string id, InvitationRequest invitationRequest)
        {
            Invitation invitationReceived = new Invitation(id, invitationRequest);
            _invitations.ReplaceOne(i => i.Id == id, invitationReceived);
            return GetFromInvitationId(id);
        }

        public void RemoveInvitationWithId(string id)
        {
            _invitations.DeleteOne(i => i.Id == id);
        }

        public Invitation AcceptInvitation(string id) 
        { 
            Invitation invitationLoaded = GetFromInvitationId(id);
            if (invitationLoaded == null)
            {
                return null;
            }
            string from = invitationLoaded.From;
            invitationLoaded.From = invitationLoaded.To;
            invitationLoaded.To = from;
            invitationLoaded.Status = InvitationStatus.Accepted;
            _invitations.ReplaceOne(i => i.Id == invitationLoaded.Id, invitationLoaded);
            return GetFromInvitationId(id); 
        }

        public Invitation RefuseInvitation(string id)
        {
            Invitation invitationLoaded = GetFromInvitationId(id);
            if (invitationLoaded == null)
                return null;
            if (invitationLoaded.Status == InvitationStatus.Pending) 
            {
                string from = invitationLoaded.From;
                invitationLoaded.From = invitationLoaded.To;
                invitationLoaded.To = from;
                invitationLoaded.Status = InvitationStatus.Refused;
                _invitations.ReplaceOne(i => i.Id == id, invitationLoaded);
                return invitationLoaded;
            }
            return null;
        }
    }
}
