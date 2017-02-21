Git is free software. Git is a version control system.
git has a mutable index called stage.
git tracks changed adc.
123
new branch is ok feature1
dev

NWJS

cordova

electron


git learn

https://www.git-tower.com/learn/git/ebook/en/command-line/branching-merging/stashing#start


nodejs安装
http://nodejs.org/download/，



select [列名],case when [類型]='varchar' or [類型]='nvarchar' then [類型]+'('+cast([長度] as nvarchar)+')' 
else (case when [類型]='numeric' or [類型]='decimal'  then [類型]+'('+cast([長度] as nvarchar)+','+cast([小數] as nvarchar)+')'  else [類型] end ) end as [類型] ,
[列说明],[標識]
--,[長度],[小數]
from
(
select 
    [表名]=c.Name,
    [表说明]=isnull(f.[value],''),
    [列名]=a.Name,
    [列序號]=a.Column_id,
    [標識]=case when is_identity=1 then '√' else '' end,
    --[主鍵]=case when exists(select 1 from sys.objects 
				--		where parent_object_id=a.object_id 
				--		and type=N'PK' and name in
    --                (select Name from sys.indexes where index_id in
    --                (select indid from sysindexkeys where and colid=a.column_id)))
    --                then '√' else '' end,
    [類型]=b.Name,
    [字節數]=case when a.[max_length]=-1 and b.Name!='xml' then 'max/2G' 
            when b.Name='xml' then ' 2^31-1字節/2G'
            else rtrim(a.[max_length]) end,
    [長度]=ColumnProperty(a.object_id,a.Name,'Precision'),
    [小數]=isnull(ColumnProperty(a.object_id,a.Name,'Scale'),0),
    [是否為空]=case when a.is_nullable=1 then '√' else '' end,
    [列说明]=isnull(e.[value],''),
    [默認值]=isnull(d.text,'')    
from 
    sys.columns a
left join
    sys.types b on a.user_type_id=b.user_type_id
inner join
    sys.objects c on a.object_id=c.object_id and c.Type='U'
left join
    syscomments d on a.default_object_id=d.ID
left join
    sys.extended_properties e on e.major_id=c.object_id and e.minor_id=a.Column_id and e.class=1 
left join
    sys.extended_properties f on f.major_id=c.object_id and f.minor_id=0 and f.class=1
	) ta 
	where ta.表名='ProcTask'

	
	nativescript anzhuang,
 http://www.bubuko.com/infodetail-677924.html
 http://www.cnblogs.com/zhongxinWang/p/4329035.html
 
 Bootstrap 加上electron
 
 Cordova, Ionic, AngularJS
 
 Vue.js he angularjs
 
 E:\Item\iRMS
 
 8888
 
 limiansuoyoudeneirong,
 
 吴华聪 web api
 http://www.cnblogs.com/wuhuacong/p/5828038.html
 
 （ Artech 和 张善友 ）
http://www.cnblogs.com/shanyou/archive/2012/09/06/2674234.html
http://www.cnblogs.com/artech/p/3506553.html

http://www.cnblogs.com/landeanfen/p/5501487.html

xianshidenglu,
jquery moblile rumendaojiangtong
http://download.csdn.net/detail/n2dx83uc2/7929977#comment

google de tiaoshi,

Responsive Admin Template

bazhengshikugeigaidiaol,ebook/en/command-line/branching-merging/stashing#start


http://www.jb51.net/article/88168.htm

chaxunyaochaxunnaxie,


Curo - Admin Template

F:\NewIRMS\IRMS\iRMS\Lib\MBPromotionGenius.dll

bb29712222


前端的路
http://www.zhihu.com/question/34388831

韵chen
http://tian1678.com/index.php/user/register/*ntZHlK7Z6h3E97U
http://tian1678.com/index.php/user/login

--
：jsjscc.info，备用域名：jsjsdd.info 
876075089

深圳寫字樓電話新總機號碼 :0755-22916676

1，查号：*4 挂机（挂机后会响铃 并在屏上显示分机号）

2，同事不再位置上，抢听：#1

3，转接：轻按挂机键 按分机号  后挂机

4，拨外线：9+号码

5，拨内线：直拨分机



\\192.168.122.2\Hroot\Public\Software\驱动



EDraw Max

git learn
http://www.open-open.com/lib/view/open1420508778375.html



important
learn nodejs
http://javascriptissexy.com/learn-node-js-completely-and-with-confidence/


悟透js
http://www.cnblogs.com/leadzen/archive/2008/02/25/1073404.html



front-end dev skills
http://lib.csdn.net/kittyjie/365772/chart/Front-End%20Dev%20Skills



