package com.inkey.core.daos.impl;

import java.sql.ResultSet;
import java.sql.SQLException;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.RowMapper;
import org.springframework.stereotype.Component;

import com.inkey.core.daos.IUserDao;
import com.inkey.core.pojo.SyUser;

@Component 
public class UserDao implements IUserDao {

	@Autowired
	private JdbcTemplate jdbcTemplate = null;
	
    class UserRowMapper implements RowMapper<SyUser> {
        //实现ResultSet到User实体的转换
        public SyUser mapRow(ResultSet rs, int rowNum) throws SQLException {
        	SyUser m = new SyUser();
            m.setUserId(rs.getInt("user_id"));
            m.setAccount(rs.getString("account"));
            return m;
        }
    };


    public SyUser GetUserById(int userId)
    {
    	return jdbcTemplate.queryForObject("SELECT * from sy_users where user_id = ?", new UserRowMapper(), 1);
    }
}