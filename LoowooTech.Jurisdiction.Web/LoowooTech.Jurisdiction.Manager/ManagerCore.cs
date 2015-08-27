using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Manager
{
    public class ManagerCore
    {
        public static ManagerCore Instance = new ManagerCore();

        private ADManager _adManager;
        public ADManager ADManager
        {
            get
            { 
                return _adManager == null ? _adManager = new ADManager() : _adManager;
            }
            private set
            {
                _adManager = value;
            }
        }


        private UserManager _userManager;
        public UserManager UserManager
        {
            get { return _userManager == null ? _userManager = new UserManager() : _userManager; }
        }


        private GroupManager _groupManager;
        public GroupManager GroupManager
        {
            get { return _groupManager == null ? _groupManager = new GroupManager() : _groupManager; }
        }
        private DataBookManager _databookManager;
        public DataBookManager DataBookManager
        {
            get { return _databookManager == null ? _databookManager = new DataBookManager() : _databookManager; }
        }

        private AUserManager _aUserManager;
        public AUserManager AUserManager
        {
            get { return _aUserManager == null ? _aUserManager = new AUserManager() : _aUserManager; }
        }
        private IgnoreManager _ignoreManager;
        public IgnoreManager IgnoreManager
        {
            get { return _ignoreManager == null ? _ignoreManager = new IgnoreManager() : _ignoreManager; }
        }
    }
}
