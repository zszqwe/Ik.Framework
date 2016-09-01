package com.inkey.maozhuan;

import java.text.DateFormat;
import java.util.Date;
import java.util.Locale;
import java.util.Map;
import java.util.Map.Entry;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;

import com.inkey.core.pojo.SyUser;
import com.inkey.core.services.IUserService;

/**
 * Handles requests for the application home page.
 */
@Controller
@RequestMapping("/api")
public class HomeController {
	
	private static final Logger logger = LoggerFactory.getLogger(HomeController.class);
	
	@Autowired
	private IUserService userService;
	
	/**
	 * Simply selects the home view to render by returning its name.
	 */
	@RequestMapping(value = "/",method = RequestMethod.GET)
	public String home(Locale locale, Model model) {
		logger.info("Welcome home! The client locale is {}.", locale);
		
		Date date = new Date();
		DateFormat dateFormat = DateFormat.getDateTimeInstance(DateFormat.LONG, DateFormat.LONG, locale);
		
		String formattedDate = dateFormat.format(date);
		
		model.addAttribute("serverTime", formattedDate );
		
		Map<String, Object> map = model.asMap();
		for(Entry<String, Object> entry :map.entrySet())
		{
			logger.info("key:{},value:{}", entry.getKey(),entry.getValue());
		}
		return "home";
	}
	
	
	@RequestMapping(value = "/getUserInfo", method = RequestMethod.GET)
	public String getUserInfo(String key,Model model) {
		
		SyUser user = userService.GetUserByKey(1);
		model.addAttribute("user",user);
		return "api_home";
	}
}
