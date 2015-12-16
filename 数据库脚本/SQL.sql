-- --------------------------------------------------------
-- 主机:                           10.22.102.19
-- 服务器版本:                        5.1.73-community - MySQL Community Server (GPL)
-- 服务器操作系统:                      Win64
-- HeidiSQL 版本:                  9.3.0.4984
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 导出 jurisdiction 的数据库结构
CREATE DATABASE IF NOT EXISTS `jurisdiction` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `jurisdiction`;


-- 导出  表 jurisdiction.authorizes 结构
CREATE TABLE IF NOT EXISTS `authorizes` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `GroupName` varchar(1023) DEFAULT NULL,
  `Manager` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。


-- 导出  表 jurisdiction.databooks 结构
CREATE TABLE IF NOT EXISTS `databooks` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `GroupName` varchar(255) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `Checker` varchar(255) DEFAULT NULL,
  `Status` int(2) DEFAULT NULL,
  `CheckTime` datetime DEFAULT NULL,
  `MaturityTime` datetime DEFAULT NULL,
  `Reason` varchar(255) DEFAULT NULL,
  `Label` bit(1) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。


-- 导出  表 jurisdiction.record 结构
CREATE TABLE IF NOT EXISTS `record` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Time` datetime DEFAULT NULL,
  `Flag` bit(2) DEFAULT NULL,
  `Mark` varchar(1023) DEFAULT NULL,
  `DID` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
