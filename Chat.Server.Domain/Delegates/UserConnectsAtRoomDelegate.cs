using Chat.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Server.Domain.Delegates
{
	public delegate void UserConnectsAtRoomDelegate(Guid connectionUid, Client client);
}
