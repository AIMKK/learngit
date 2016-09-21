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

现货的理解
http://wenku.baidu.com/link?url=WM1ce6y1fYHMiReY91hkYxJD1NdaZiptkRBou2L5UnbFOqCfQnMw1eBuvojvqnR17mwLWLOodzFUN70PEAgBFgrWwj86fkXI2VYNFHcwyNW

nodejs安装
http://nodejs.org/download/，

对冲机制是什么意思

做市商模式的定义，
通俗定义：同时向市场提供双向的买卖报价。

上海期货交易所

JavaScript入门经典(第4版)英文版

JavaScript高级程序设计第3版


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
 
 