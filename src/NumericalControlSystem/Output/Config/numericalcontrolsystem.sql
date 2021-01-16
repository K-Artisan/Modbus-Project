/*
Navicat MySQL Data Transfer

Source Server         : Test
Source Server Version : 50523
Source Host           : localhost:3306
Source Database       : numericalcontrolsystem

Target Server Type    : MYSQL
Target Server Version : 50523
File Encoding         : 65001

Date: 2013-08-19 22:55:44
*/

-- ----------------------------
-- CREATE SCHEMA `numericalcontrolsystem`
-- ----------------------------

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `datapoint`
-- ----------------------------
DROP TABLE IF EXISTS `datapoint`;
CREATE TABLE `datapoint` (
  `DataPointId` int(11) NOT NULL AUTO_INCREMENT,
  `Number` int(11) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `DeviceAddress` int(11) DEFAULT NULL,
  `StartRegisterAddress` int(11) NOT NULL,
  `DataType` int(11) NOT NULL,
  `DataPointType` int(11) NOT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `ModuleId` int(11) NOT NULL,
  PRIMARY KEY (`DataPointId`),
  UNIQUE KEY `Id_UNIQUE` (`Number`),
  KEY `FK_datapoint_module` (`ModuleId`),
  KEY `FK_datapoint_belongto_module` (`ModuleId`),
  CONSTRAINT `FK_datapoint_belongto_module` FOREIGN KEY (`ModuleId`) REFERENCES `module` (`ModuleId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=957 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of datapoint
-- ----------------------------

-- ----------------------------
-- Table structure for `datapointhistorydata`
-- ----------------------------
DROP TABLE IF EXISTS `datapointhistorydata`;
CREATE TABLE `datapointhistorydata` (
  `DataPointHistoryDataId` varchar(50) NOT NULL,
  `DataPointId` int(11) NOT NULL,
  `DateTime` datetime NOT NULL,
  `Value` double NOT NULL,
  PRIMARY KEY (`DataPointHistoryDataId`),
  KEY `FKDatapointvalue2013070800RefDataPoint` (`DataPointId`),
  KEY `FK_datapointhistorydata_ref_datapoint_datapointid` (`DataPointId`),
  CONSTRAINT `FK_datapointhistorydata_belongto_datapoint_datapointid` FOREIGN KEY (`DataPointId`) REFERENCES `datapoint` (`DataPointId`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of datapointhistorydata
-- ----------------------------

-- ----------------------------
-- Table structure for `module`
-- ----------------------------
DROP TABLE IF EXISTS `module`;
CREATE TABLE `module` (
  `ModuleId` int(11) NOT NULL AUTO_INCREMENT,
  `Number` int(11) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Description` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`ModuleId`),
  UNIQUE KEY `Number_UNIQUE` (`Number`)
) ENGINE=InnoDB AUTO_INCREMENT=158 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of module
-- ----------------------------
