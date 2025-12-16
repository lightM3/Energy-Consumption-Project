-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: localhost    Database: enerjitahmindb
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Temporary view structure for view `vw_userdetails`
--

DROP TABLE IF EXISTS `vw_userdetails`;
/*!50001 DROP VIEW IF EXISTS `vw_userdetails`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_userdetails` AS SELECT 
 1 AS `UserID`,
 1 AS `MaskedName`,
 1 AS `MaskedEmail`,
 1 AS `MaskedPassword`,
 1 AS `RoleName`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_latestpredictions`
--

DROP TABLE IF EXISTS `vw_latestpredictions`;
/*!50001 DROP VIEW IF EXISTS `vw_latestpredictions`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_latestpredictions` AS SELECT 
 1 AS `TargetDate`,
 1 AS `TargetTime`,
 1 AS `PredictedValue`,
 1 AS `CreatedDate`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_highconsumptionhours`
--

DROP TABLE IF EXISTS `vw_highconsumptionhours`;
/*!50001 DROP VIEW IF EXISTS `vw_highconsumptionhours`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_highconsumptionhours` AS SELECT 
 1 AS `ConsumptionID`,
 1 AS `Date`,
 1 AS `Time`,
 1 AS `MWh_Value`,
 1 AS `Lag_24h`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_ml_model_hazirlik`
--

DROP TABLE IF EXISTS `vw_ml_model_hazirlik`;
/*!50001 DROP VIEW IF EXISTS `vw_ml_model_hazirlik`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_ml_model_hazirlik` AS SELECT 
 1 AS `DateTime`,
 1 AS `Tuketim_MWh`,
 1 AS `Ankara_Sicaklik`,
 1 AS `Erzurum_Sicaklik`,
 1 AS `Istanbul_Sicaklik`,
 1 AS `Izmir_Sicaklik`,
 1 AS `Weighted_Avg_Temp`,
 1 AS `Saat`,
 1 AS `Ay`,
 1 AS `Yil`,
 1 AS `Haftanin_Gunu`,
 1 AS `Hafta_Sonu`,
 1 AS `Mesai_Saati`,
 1 AS `Lag_Tuketim_24h`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_dailyconsumptionsummary`
--

DROP TABLE IF EXISTS `vw_dailyconsumptionsummary`;
/*!50001 DROP VIEW IF EXISTS `vw_dailyconsumptionsummary`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_dailyconsumptionsummary` AS SELECT 
 1 AS `Date`,
 1 AS `TotalConsumption`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_cityaveragetemperature`
--

DROP TABLE IF EXISTS `vw_cityaveragetemperature`;
/*!50001 DROP VIEW IF EXISTS `vw_cityaveragetemperature`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_cityaveragetemperature` AS SELECT 
 1 AS `CityName`,
 1 AS `AvgTemp`*/;
SET character_set_client = @saved_cs_client;

--
-- Final view structure for view `vw_userdetails`
--

/*!50001 DROP VIEW IF EXISTS `vw_userdetails`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_userdetails` AS select `u`.`UserID` AS `UserID`,concat(left(`u`.`FullName`,2),'****') AS `MaskedName`,concat(left(`u`.`Email`,2),'****',substr(`u`.`Email`,locate('@',`u`.`Email`))) AS `MaskedEmail`,'********' AS `MaskedPassword`,`r`.`RoleName` AS `RoleName` from (`users` `u` join `roles` `r` on((`u`.`RoleID` = `r`.`RoleID`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_latestpredictions`
--

/*!50001 DROP VIEW IF EXISTS `vw_latestpredictions`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_latestpredictions` AS select `predictions`.`TargetDate` AS `TargetDate`,`predictions`.`TargetTime` AS `TargetTime`,`predictions`.`PredictedValue` AS `PredictedValue`,`predictions`.`CreatedDate` AS `CreatedDate` from `predictions` order by `predictions`.`CreatedDate` desc */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_highconsumptionhours`
--

/*!50001 DROP VIEW IF EXISTS `vw_highconsumptionhours`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_highconsumptionhours` AS select `electricityconsumptions`.`ConsumptionID` AS `ConsumptionID`,`electricityconsumptions`.`Date` AS `Date`,`electricityconsumptions`.`Time` AS `Time`,`electricityconsumptions`.`MWh_Value` AS `MWh_Value`,`electricityconsumptions`.`Lag_24h` AS `Lag_24h` from `electricityconsumptions` where (`electricityconsumptions`.`MWh_Value` > 30000) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_ml_model_hazirlik`
--

/*!50001 DROP VIEW IF EXISTS `vw_ml_model_hazirlik`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_ml_model_hazirlik` AS select timestamp(`e`.`Date`,`e`.`Time`) AS `DateTime`,`e`.`MWh_Value` AS `Tuketim_MWh`,`w1`.`Temperature` AS `Ankara_Sicaklik`,`w2`.`Temperature` AS `Erzurum_Sicaklik`,`w3`.`Temperature` AS `Istanbul_Sicaklik`,`w4`.`Temperature` AS `Izmir_Sicaklik`,round(((((`w3`.`Temperature` * 0.40) + (`w1`.`Temperature` * 0.25)) + (`w4`.`Temperature` * 0.25)) + (`w2`.`Temperature` * 0.10)),2) AS `Weighted_Avg_Temp`,hour(`e`.`Time`) AS `Saat`,month(`e`.`Date`) AS `Ay`,year(`e`.`Date`) AS `Yil`,weekday(`e`.`Date`) AS `Haftanin_Gunu`,(case when (weekday(`e`.`Date`) >= 5) then 1 else 0 end) AS `Hafta_Sonu`,(case when ((weekday(`e`.`Date`) < 5) and (hour(`e`.`Time`) between 9 and 17)) then 1 else 0 end) AS `Mesai_Saati`,`e`.`Lag_24h` AS `Lag_Tuketim_24h` from ((((`electricityconsumptions` `e` left join `weathermeasurements` `w1` on(((`e`.`Date` = `w1`.`Date`) and (`e`.`Time` = `w1`.`Time`) and (`w1`.`CityID` = 1)))) left join `weathermeasurements` `w2` on(((`e`.`Date` = `w2`.`Date`) and (`e`.`Time` = `w2`.`Time`) and (`w2`.`CityID` = 2)))) left join `weathermeasurements` `w3` on(((`e`.`Date` = `w3`.`Date`) and (`e`.`Time` = `w3`.`Time`) and (`w3`.`CityID` = 3)))) left join `weathermeasurements` `w4` on(((`e`.`Date` = `w4`.`Date`) and (`e`.`Time` = `w4`.`Time`) and (`w4`.`CityID` = 4)))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_dailyconsumptionsummary`
--

/*!50001 DROP VIEW IF EXISTS `vw_dailyconsumptionsummary`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_dailyconsumptionsummary` AS select `electricityconsumptions`.`Date` AS `Date`,sum(`electricityconsumptions`.`MWh_Value`) AS `TotalConsumption` from `electricityconsumptions` group by `electricityconsumptions`.`Date` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_cityaveragetemperature`
--

/*!50001 DROP VIEW IF EXISTS `vw_cityaveragetemperature`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_cityaveragetemperature` AS select `c`.`CityName` AS `CityName`,avg(`w`.`Temperature`) AS `AvgTemp` from (`weathermeasurements` `w` join `cities` `c` on((`w`.`CityID` = `c`.`CityID`))) group by `c`.`CityName` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-12-16 14:00:37
