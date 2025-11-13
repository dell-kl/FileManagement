INSERT INTO Usuario(
	usuario_guid,
	usuario_nombre,
	usuario_cedula,
	usuario_email,
	usuario_celular,
	usuario_passwd,
	usuario_estado,
	RolId,
	SucursalId,
	usuario_nintentos
)
VALUES(
	NEWID(),
	'SRG.TEC.GABRIEL CARAGULLA',
	'1102935341',
	'gbGabriel@hotmail.com',
	'1111111111',
	'CfDJ8PxbiB83aYZGozahOzmOC-wSm0HjP8MYruuptQUO1whOTfplNo07H7Y9O_BDxbvfMHERvChuAeQddTufFgf4Q2l_k2h3lg7TzWeecQ_44QZbehbrXVzN6JWdTBHRULganQ',
	'activo',
	1,
	1,
	3
);