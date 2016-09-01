package com.inkey.core.services.impl;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.inkey.core.daos.IUserDao;
import com.inkey.core.daos.IUserDaoMaper;
import com.inkey.core.pojo.SyUser;
import com.inkey.core.services.IUserService;

@Service
public class UserService implements IUserService {

	@Autowired
	private IUserDao userDao = null;
	
	@Autowired
	private IUserDaoMaper userDaoMaper;
	
	public SyUser GetUserById(int userId) {
		SyUser u = userDao.GetUserById(1);
		return u;
	}

	public SyUser GetUserByKey(int userId) {
		//SyUser u = null;
		SyUser u = userDaoMaper.GetUserById(1);
		return u;
	}
}
