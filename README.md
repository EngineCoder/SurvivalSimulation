# Survival Simulation
#### 简介
**以Unity3D做客户端，Photon做服务端开发一款多人联机生存经营类游戏。**  
***

# Photon Server
#### 简介
***
+ **[Photon 官网](https://www.photonengine.com/)**
+ **[Photon Server Intro]https://doc.photonengine.com/zh-cn/server/current/getting-started/photon-server-intro

#### 安装教程  

+ Unity  
1、[Unity3D Download](https://unity.com/cn/download)  
2、[Unity3D Documentation](https://docs.unity.cn/cn/2023.2/Manual/UnityManual.html)  

+ Visual Studio  
3、[Visual Studio Download](https://visualstudio.microsoft.com/zh-hans/downloads/)  
4、[Visual Studio Tutorial](https://docs.microsoft.com/zh-cn/) 

+ Photon  
1、[Photon Download](https://www.photonengine.com/zh-CN/sdks#server-sdkserverserver)  
2、[Photon Documentation](https://doc.photonengine.com/en-us/realtime/current/getting-started/realtime-intro)  
3、[Photon API](https://doc-api.photonengine.com/)  
4、[Photon PUN2](https://doc.photonengine.com/en-us/pun/v2/getting-started/pun-intro)  

+ NHibernate  
1、[NHibernate Download](https://nhibernate.info/)  
2、[NHibernate Documentation](https://nhibernate.info/doc/index.html)

+ Mysql  
1、[Mysql Download](https://dev.mysql.com/downloads/installer/)  
2、[Mysql Documentation](https://dev.mysql.com/doc/)

+ Other  
1、[System.IO - FileModel](https://www.cnblogs.com/OpenCoder/p/10766522.html)  


#### 二、配置部署   

1、开发的服务端生成的类库（.dll）要部署在该文件夹下   
![](https://images.gitee.com/uploads/images/2021/0408/221042_dc3d88c7_809545.png "屏幕截图.png")   
2、运行PhotonControl.exe   
![](https://images.gitee.com/uploads/images/2021/0408/222936_7b483061_809545.png "屏幕截图.png")   
3、服务器配置
![](https://images.gitee.com/uploads/images/2021/0408/224536_391bd944_809545.png "屏幕截图.png")   
4、配置1
![](https://images.gitee.com/uploads/images/2021/0413/173927_a49c1216_809545.png "屏幕截图.png")   
5、配置2
![](https://images.gitee.com/uploads/images/2021/0413/174732_f75f2b58_809545.png "屏幕截图.png")   
4、新建.Framework类库，引入Photon中lib文件夹下的dll   
![](https://images.gitee.com/uploads/images/2021/0414/001203_89c76811_809545.png "屏幕截图.png")
***

#### 使用教程  

1、需要安装Mysql，使用Mysql可视化编辑工具执行根目录下的Database.sql文件快速创建数据库。  
2、运行根目录下Server/deploy/bin_Win64/文件夹下的PhotonControl.exe  
3、先使用PhotonControl控制面板中的Game Server IP Config 配置一下服务器IP地址。  
4、最后启动已配置好的 Survial 服务器，Start as application,启动游戏服务器。如下图  
![](https://images.gitee.com/uploads/images/2020/0921/134921_7bb98680_809545.png "屏幕截图.png")     
5、打开已下载的Unity3D工程，运行，注册，登录，就可以试玩了。    

#### 主要功能

+ 地图系统  
随机地形的生成。（树木、草地、湖泊、河流、道路、芦苇、沼泽等等）  
+ 背包系统   
+ 锻造系统   
锻造武器（弓箭，弩，枪，工具(绳子、木板、木箱等等)、战车、家用汽车、摩托车等等）
+ 采集系统   
采集材料（石头、树木、动物毛皮、芦苇等等）。  
+ 砍伐系统   
+ 掉落系统   
+ 开采系统   
+ 拾取系统  
+ 建造系统  
想怎么建就怎么建、可以建造房屋、各种牲畜圈养栏、自己的店铺、公共集市、卖一些材料物品  
+ 动物系统  
动物随机生成，逼真的AI(逃跑、攻击、死亡、巡逻等等)，注意生活习性确保生成的区域，绝大部分生活在合适的区域，个别的可能会跑出区域范围。（兔子、猪、羊、老虎、熊、牛、鱼）  
+ 热更系统
+ 房间系统  
+ 匹配系统  
+ 同步系统  
+ 钓鱼系统
+ 种植系统  
+ 成长系统  

#### 参与贡献
***
***
***
***