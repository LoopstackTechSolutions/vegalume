create database dbvegalume;
use dbvegalume;

-- Tabela que armazena os pratos disponíveis no sistema
create table tb_prato 
(
    idPrato int auto_increment primary key,
    nomePrato varchar(35) unique not null,
    precoPrato decimal(6,2) not null,
    descricaoPrato varchar(250) not null,
    valorCalorico int not null,
    peso int not null,
    pessoasServidas smallint not null,
    statusPrato bit not null
);

-- Tabela que armazena os dados dos clientes
create table tb_cliente 
(
    idCliente int auto_increment primary key,
    nome varchar(60) not null,
    senha varchar(30) not null,
    telefone bigint not null,
    email varchar(50) unique not null
);

-- Tabela que armazena os dados dos cartões de pagamento
create table tb_cartao
(
    idCartao int primary key auto_increment,
    numeroCartao bigint,
    nomeTitular varchar(30) not null,
    dataValidade date not null,
    cvv smallint not null,
    modalidade bit not null,
    bandeira varchar(30) not null,
    idCliente int,
    foreign key (idCliente) references tb_cliente (idCliente)
);

-- Tabela com os endereços cadastrados
create table tb_endereco 
(
    idEndereco int auto_increment primary key,
    estado char(2) not null,
    cidade varchar(50) not null,
    bairro varchar(50) not null,
    rua varchar(50) not null,
    numero int not null,
    idCliente int,
    foreign key (idCliente) references tb_cliente (idCliente)
);

-- Tabela que armazena os dados dos funcionários do sistema
create table tb_funcionario (
    rm smallint auto_increment primary key,
    nome varchar(60) not null,
    senha varchar(30) not null,
    telefone bigint not null,
    email varchar(50) unique not null
);

-- Tabela que registra os pedidos feitos pelos clientes
create table tb_pedido 
(
    idPedido int auto_increment primary key,
    statusPedido varchar(20) default "espera", -- "espera"/"preparacao"/"transito"/"entregue"/"cancelado"
    dataHoraPedido timestamp default current_timestamp(),
    valorTotal decimal(8,2),
    rm smallint default null,
    idEndereco int not null,
    idCliente int not null,
    idCartao int,
    foreign key (rm) references tb_funcionario (rm),
    foreign key (idEndereco) references tb_endereco (idEndereco),
    foreign key (idCliente) references tb_cliente (idCliente),
    foreign key (idCartao) references tb_cartao (idCartao)
);

-- Relação entre pratos e pedidos (cada pedido pode ter vários pratos e vice-versa)
create table tb_prato_pedido 
(
    idPrato int,
    idPedido int,
    qtd int not null,
    detalhesPedido varchar(200),
    primary key (idPrato, idPedido),
    foreign key (idPrato) references tb_prato (idPrato),
    foreign key (idPedido) references tb_pedido (idPedido)
);

insert into tb_cliente (nome, senha, telefone, email) values 
('Vinicius', 12345678, 11948577155, 'vinicavequi@gmail.com'),
('Lucas', 12345678, 11948577155, 'lucas@gmail.com'),
('Halisson', 12345678, 11948577155, 'halisson@gmail.com'),
('Matheus', 12345678, 11948577155, 'matheus@gmail.com'),
('Marco', 12345678, 11948577155, 'marco@gmail.com');

insert into tb_endereco (rua, numero, bairro, cidade, estado, idcliente) values
('Rua Feliz', 44, 'Vila Feliz', 'Felizândia', 'SP', 1);

insert into tb_funcionario (nome, senha, telefone, email) values ('Vinicius', 12345678, 11948577155, 'vinicavequi@gmail.com');

insert into tb_prato (nomePrato, precoPrato, descricaoPrato, valorCalorico, peso, pessoasServidas, statusPrato) values
('Falafel', 19, 'Bolinho crocante de grão-de-bico temperado com ervas e especiarias, servido com molho tahine.', 320, 200, 1, 1),
('Estrogonofe de Cogumelos', 30, 'Estrogonofe cremoso feito com cogumelos frescos e creme de castanhas, acompanhado de arroz integral e batata palha.', 480, 400, 1, 1),
('Feijoada Vegana', 35, 'Feijoada feita com legumes, cogumelos, tofu defumado e acompanhamentos tradicionais como arroz, couve e farofa.', 600, 500, 2, 1),
('Lasanha de Berinjela', 27, 'Lasanha de camadas de berinjela grelhada, molho de tomate e ricota de tofu temperada.', 410, 350, 1, 0),
('Ratatouille', 25, 'Ensopado de legumes mediterrâneos como berinjela, abobrinha, pimentão e tomate, temperado com ervas finas.', 350, 300, 1, 1),
('Gazpacho', 22, 'Sopa fria de tomate, pimentão, pepino e cebola, refrescante para dias quentes.', 150, 250, 1, 1),
('Samosa', 18, 'Pastelzinho frito recheado com batata, ervilhas, cenoura e especiarias.', 280, 150, 1, 1);

insert into tb_pedido (idEndereco, idCliente, idCartao, valorTotal) values (1, 1, null, 90);
insert into tb_prato_pedido (idPrato, idPedido, qtd, detalhesPedido) values (6, 1, 1, null), (3, 1, 2, 'tempere bem!');
insert into tb_pedido (idEndereco, idCliente, idCartao, valorTotal) values (1, 1, null, 120);
insert into tb_prato_pedido (idPrato, idPedido, qtd, detalhesPedido) values (2, 2, 1, 'SEM BATATA PALHA!!'), (1, 2, 2, null);

insert into tb_pedido (idEndereco, idCliente, idCartao, valorTotal) values (1, 2, null, 120);
insert into tb_prato_pedido (idPrato, idPedido, qtd, detalhesPedido) values (2, 3, 1, 'SEM BATATA PALHA!!'), (1, 3, 2, null);