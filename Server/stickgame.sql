-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: stickgame
-- ------------------------------------------------------
-- Server version	5.7.17-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `room`
--

DROP TABLE IF EXISTS `room`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `room` (
  `id` int(10) NOT NULL COMMENT '唯一ID',
  `room_type` int(1) NOT NULL COMMENT '房间类型',
  `room_name` varchar(45) NOT NULL COMMENT '房间名称',
  `room_owner` bigint(20) NOT NULL COMMENT '房主',
  `room_players` varchar(45) NOT NULL COMMENT '玩家',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `room`
--

LOCK TABLES `room` WRITE;
/*!40000 ALTER TABLE `room` DISABLE KEYS */;
INSERT INTO `room` VALUES (1,1,'1',1,'1,2,3,4,5,6,7,8,9,10');
/*!40000 ALTER TABLE `room` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user` (
  `id` bigint(20) NOT NULL COMMENT '唯一ID',
  `user_account` varchar(20) NOT NULL COMMENT '账号',
  `user_password` varchar(20) NOT NULL COMMENT '密码',
  `user_nickname` varchar(14) NOT NULL COMMENT '昵称',
  `user_integral_grade` varchar(10) DEFAULT NULL COMMENT '积分/等级',
  `user_sex` int(1) DEFAULT NULL COMMENT '性别',
  `user_copper_coins` int(10) DEFAULT NULL COMMENT '铜币',
  `user_gold_ingot` int(10) DEFAULT NULL COMMENT '金锭',
  `user_innings` int(10) DEFAULT NULL COMMENT '局数/场数',
  `user_win_rate` int(3) DEFAULT NULL COMMENT '胜率',
  `user_lastlogin_date` datetime DEFAULT NULL COMMENT '最后一次登录日期',
  `user_register_date` datetime DEFAULT NULL COMMENT '注册日期',
  PRIMARY KEY (`id`),
  UNIQUE KEY `user_id_UNIQUE` (`user_account`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'1','1','UFO1号','520',0,20000,10000,0,0,'2021-07-29 12:25:20','2020-06-22 15:05:14'),(2,'2','2','UFO2号','364',0,0,0,0,0,'2021-06-28 02:18:11','2020-06-23 17:28:08'),(3,'3','3','UFO3号','385',0,0,0,0,0,'2021-06-28 02:18:11','2020-06-30 07:41:05'),(4,'4','4','UFO4号','500',0,0,0,0,0,'2021-06-28 02:18:14','2020-09-18 06:37:24'),(5,'5','5','UFO5号','620',0,0,0,0,0,'2021-06-28 02:18:16','2020-09-18 06:57:46'),(6,'6','6','UFO6号','850',0,0,0,0,0,'2021-06-28 02:18:18','2020-09-18 06:59:06'),(7,'7','7','UFO7号','670',0,0,0,0,0,'2021-06-28 02:18:21','2020-09-18 06:59:51'),(8,'8','8','UFO8号','123',0,0,0,0,0,'2021-06-28 02:18:25','2020-09-18 07:00:32'),(9,'9','9','UFO9号','800',0,0,0,0,0,'2021-06-28 02:18:29','2020-09-18 07:01:31'),(10,'10','10','UFO10号','1200',0,0,0,0,0,'2021-06-28 02:18:32','2020-09-18 07:02:25');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-08-02 23:05:29
