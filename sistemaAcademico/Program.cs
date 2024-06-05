using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Pessoa {
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public DateTime DataNascimento { get; set; }

    public Pessoa(string nome, string cpf, DateTime dataNascimento) {
        Nome = nome;
        Cpf = cpf;
        DataNascimento = dataNascimento;
    }
}

public class Aluno : Pessoa {
    public int Matricula { get; set; }
    [JsonIgnore]
    public Turma? Turma { get; set; }

    public Aluno(string nome, string cpf, DateTime dataNascimento, int matricula, Turma turma) 
        : base(nome, cpf, dataNascimento) {
        Matricula = matricula;
        Turma = turma;
    }

    [JsonConstructor]
    public Aluno(string nome, string cpf, DateTime dataNascimento, int matricula)
        : base(nome, cpf, dataNascimento) {
        Matricula = matricula;
        Turma = null;
    }

    public float CalcularMedia() {
        return 0.0f;
    }
}

public class Professor : Pessoa {
    public int Registro { get; set; }

    public Professor(string nome, string cpf, DateTime dataNascimento, int registro) 
        : base(nome, cpf, dataNascimento) {
        Registro = registro;
    }
}

public class Prova {
    private Dictionary<int, float> notas = new Dictionary<int, float>();

    public float GetNota(int matricula) {
        return notas.ContainsKey(matricula) ? notas[matricula] : 0.0f;
    }

    public void SetNota(int matricula, float nota) {
        notas[matricula] = nota;
    }
}

public class Disciplina {
    public string Nome { get; set; }
    public int CargaHoraria { get; set; }
    public Professor Professor { get; set; }
    
    public Prova Prova1 { get; set; }
    public Prova Prova2 { get; set; }

    public Disciplina(string nome, int cargaHoraria, Professor professor) {
        Nome = nome;
        CargaHoraria = cargaHoraria;
        Professor = professor;
        Prova1 = new Prova();
        Prova2 = new Prova();
    }
}

public class Turma {
    public int AnoDaTurma { get; set; }
    public List<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();
    public List<Aluno> Alunos { get; set; } = new List<Aluno>();

    public Turma(int anoDaTurma) {
        AnoDaTurma = anoDaTurma;
    }

    public void AddDisciplina(Disciplina disciplina) {
        Disciplinas.Add(disciplina);
    }

    public void AddAluno(Aluno aluno) {
        Alunos.Add(aluno);
    }
}

public class SistemaAcademico {
    private const string FileName = "dados.json";
    private static List<Turma> turmas = new List<Turma>();

    private static void SalvarDados() {
        var options = new JsonSerializerOptions { 
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };
        string jsonString = JsonSerializer.Serialize(turmas, options);
        File.WriteAllText(FileName, jsonString);
    }

    private static void CarregarDados() {
        if (File.Exists(FileName)) {
            var options = new JsonSerializerOptions {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string jsonString = File.ReadAllText(FileName);
            var deserializedTurmas = JsonSerializer.Deserialize<List<Turma>>(jsonString, options);
            turmas = deserializedTurmas ?? new List<Turma>();

            foreach (var turma in turmas) {
                foreach (var aluno in turma.Alunos) {
                    aluno.Turma = turma;
                }
            }
        }
    }

    public static void Sistema() {
        char opcao = ' ';
        CarregarDados();

        while (opcao != 'l') {
            opcao = menu();            
            Console.WriteLine();

            try {
                switch (opcao) {
                    case 't':
                        Console.WriteLine("Digite o ano da turma:");
                        string? anoDaTurmaStr = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(anoDaTurmaStr) && int.TryParse(anoDaTurmaStr, out int anoDaTurma)) {
                            turmas.Add(new Turma(anoDaTurma));
                        } else {
                            Console.WriteLine("Ano inválido.");
                        }
                        break;

                    case 'd':
                        AdicionarDisciplina();
                        break;

                    case 'a':
                        AdicionarAluno();
                        break;

                    case 'n':
                        AdicionarNotas();
                        break;

                    case 'm':
                        MostrarDados();
                        break;

                    case 'l':
                        Console.WriteLine("Saindo...");
                        SalvarDados();
                        break;

                    default:
                        Console.WriteLine("Opção inválida");
                        break;
                }
            } catch (Exception ex) {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
    }

    private static char menu() {
        Console.WriteLine("Digite a opção desejada (l para sair):");
        Console.WriteLine("t - Adicionar turma");
        Console.WriteLine("d - Adicionar disciplina");
        Console.WriteLine("a - Adicionar aluno");
        Console.WriteLine("n - Adicionar notas");
        Console.WriteLine("m - Mostrar dados");
        Console.WriteLine("l - Sair");

        return Console.ReadKey().KeyChar;
    }

    private static void AdicionarDisciplina() {
        Console.WriteLine("Digite o ano da turma:");
        string? anoTurmaStr = Console.ReadLine();
        if (!int.TryParse(anoTurmaStr, out int anoTurma)) {
            Console.WriteLine("Ano inválido.");
            return;
        }

        Turma? turma = turmas.Find(t => t.AnoDaTurma == anoTurma);
        if (turma == null) {
            Console.WriteLine("Turma não encontrada");
            return;
        }

        Console.WriteLine("Digite o nome da disciplina:");
        string? nomeDisciplina = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nomeDisciplina)) {
            Console.WriteLine("Nome da disciplina inválido.");
            return;
        }

        Console.WriteLine("Digite a carga horária da disciplina:");
        string? cargaHorariaStr = Console.ReadLine();
        if (!int.TryParse(cargaHorariaStr, out int cargaHoraria)) {
            Console.WriteLine("Carga horária inválida.");
            return;
        }

        Console.WriteLine("Digite o nome do professor:");
        string? nomeProfessor = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nomeProfessor)) {
            Console.WriteLine("Nome do professor inválido.");
            return;
        }

        Console.WriteLine("Digite o CPF do professor:");
        string? cpfProfessor = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(cpfProfessor)) {
            Console.WriteLine("CPF do professor inválido.");
            return;
        }

        Console.WriteLine("Digite a data de nascimento do professor (yyyy-mm-dd):");
        string? dataNascimentoStr = Console.ReadLine();
        if (!DateTime.TryParse(dataNascimentoStr, out DateTime dataNascimento)) {
            Console.WriteLine("Data de nascimento inválida.");
            return;
        }

        Console.WriteLine("Digite o registro do professor:");
        string? registroStr = Console.ReadLine();
        if (!int.TryParse(registroStr, out int registro)) {
            Console.WriteLine("Registro inválido.");
            return;
        }

        Professor professor = new Professor(nomeProfessor, cpfProfessor, dataNascimento, registro);
        Disciplina disciplina = new Disciplina(nomeDisciplina, cargaHoraria, professor);
        turma.AddDisciplina(disciplina);
    }

    private static void AdicionarAluno() {
        Console.WriteLine("Digite o ano da turma:");
        string? anoTurmaStr = Console.ReadLine();
        if (!int.TryParse(anoTurmaStr, out int anoTurma)) {
            Console.WriteLine("Ano inválido.");
            return;
        }

        Turma? turma = turmas.Find(t => t.AnoDaTurma == anoTurma);
        if (turma == null) {
            Console.WriteLine("Turma não encontrada");
            return;
        }

        Console.WriteLine("Digite o nome do aluno:");
        string? nomeAluno = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nomeAluno)) {
            Console.WriteLine("Nome do aluno inválido.");
            return;
        }

        Console.WriteLine("Digite o CPF do aluno:");
        string? cpfAluno = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(cpfAluno)) {
            Console.WriteLine("CPF do aluno inválido.");
            return;
        }

        Console.WriteLine("Digite a data de nascimento do aluno (yyyy-mm-dd):");
        string? dataNascimentoStr = Console.ReadLine();
        if (!DateTime.TryParse(dataNascimentoStr, out DateTime dataNascimento)) {
            Console.WriteLine("Data de nascimento inválida.");
            return;
        }

        Console.WriteLine("Digite a matrícula do aluno:");
        string? matriculaStr = Console.ReadLine();
        if (!int.TryParse(matriculaStr, out int matricula)) {
            Console.WriteLine("Matrícula inválida.");
            return;
        }

        Aluno aluno = new Aluno(nomeAluno, cpfAluno, dataNascimento, matricula, turma);
        turma.AddAluno(aluno);
    }

    private static void AdicionarNotas() {
        Console.WriteLine("Digite o ano da turma:");
        string? anoTurmaStr = Console.ReadLine();
        if (!int.TryParse(anoTurmaStr, out int anoTurma)) {
            Console.WriteLine("Ano inválido.");
            return;
        }

        Turma? turma = turmas.Find(t => t.AnoDaTurma == anoTurma);
        if (turma == null) {
            Console.WriteLine("Turma não encontrada");
            return;
        }

        Console.WriteLine("Digite o nome da disciplina:");
        string? nomeDisciplina = Console.ReadLine();
        Disciplina? disciplina = turma.Disciplinas.Find(d => d.Nome.Equals(nomeDisciplina, StringComparison.OrdinalIgnoreCase));
        if (disciplina == null) {
            Console.WriteLine("Disciplina não encontrada");
            return;
        }

        Console.WriteLine("Digite a matrícula do aluno:");
        string? matriculaStr = Console.ReadLine();
        if (!int.TryParse(matriculaStr, out int matricula)) {
            Console.WriteLine("Matrícula inválida.");
            return;
        }

        Aluno? aluno = turma.Alunos.Find(a => a.Matricula == matricula);
        if (aluno == null) {
            Console.WriteLine("Aluno não encontrado");
            return;
        }

        Console.WriteLine("Digite a nota da prova 1:");
        string? nota1Str = Console.ReadLine();
        if (!float.TryParse(nota1Str, out float nota1)) {
            Console.WriteLine("Nota inválida.");
            return;
        }

        Console.WriteLine("Digite a nota da prova 2:");
        string? nota2Str = Console.ReadLine();
        if (!float.TryParse(nota2Str, out float nota2)) {
            Console.WriteLine("Nota inválida.");
            return;
        }

        disciplina.Prova1.SetNota(aluno.Matricula, nota1);
        disciplina.Prova2.SetNota(aluno.Matricula, nota2);
    }

    private static void MostrarDados() {
        foreach (var turma in turmas) {
            Console.WriteLine($"Turma: {turma.AnoDaTurma}");

            foreach (var aluno in turma.Alunos) {
                Console.WriteLine($"  Aluno: {aluno.Nome}, Matrícula: {aluno.Matricula}");
            }

            foreach (var disciplina in turma.Disciplinas) {
                Console.WriteLine($"  Disciplina: {disciplina.Nome}, Carga Horária: {disciplina.CargaHoraria}");
                Console.WriteLine($"    Professor: {disciplina.Professor.Nome}");

                foreach (var aluno in turma.Alunos) {
                    float nota1 = disciplina.Prova1.GetNota(aluno.Matricula);
                    float nota2 = disciplina.Prova2.GetNota(aluno.Matricula);
                    Console.WriteLine($"    Aluno: {aluno.Nome}, Nota Prova 1: {nota1}, Nota Prova 2: {nota2}");
                }
            }
        }
    }
}


public class MainClass {
    public static void Main() {
        SistemaAcademico.Sistema();
    }
}
