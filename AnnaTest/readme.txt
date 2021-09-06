

允许程序运行
netsh http add urlacl url=http://*:1234/ user=Everyone listen=yes

恢复规则
netsh http delete urlacl url=http://*:1234/