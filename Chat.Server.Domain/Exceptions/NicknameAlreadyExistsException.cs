﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Server.Domain.Exceptions
{
	public class NicknameAlreadyExistsException : Exception
	{
		public NicknameAlreadyExistsException()
			   : base("This nickname already exists.")
		{

		}
	}
}
