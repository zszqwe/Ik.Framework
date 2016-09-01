package com.inkey.core.services;

import com.inkey.core.pojo.SyUser;

public interface IUserService {
	 SyUser GetUserById(int userId);
	 
	 SyUser GetUserByKey(int userId);
}
