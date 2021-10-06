using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    private bool alive;
    private bool never_moved;
    private bool white;
    private GameObject position_square;
    private int square_number;
    private string type;
    private GameObject piece;

    public Piece(int position_of_pieces, string type_of_pieces, bool white_or_black, string gameObject)
    {
        this.never_moved = true;
        this.position_square = GameObject.Find("board " + position_of_pieces);
        this.square_number = position_of_pieces;
        this.type = type_of_pieces;
        this.white = white_or_black;
        this.piece = GameObject.Find(gameObject);
        this.alive = true;
    }

    public bool colorWhite()
    {
        return this.white;
    }

    public Transform get_position_square()
    {
        return this.position_square.transform.GetChild(0);
    }

    public string getSquareName()
    {
        return this.position_square.name;
    }

    public string getPieceName()
    {
        return this.type;
    }

    public bool has_never_moved_before()
    {
        return this.never_moved;
    }

    public void has_now_moved()
    {
        this.never_moved = false;
    }

    public int get_square_number()
    {
        return this.square_number;
    }

    public GameObject get_piece_GameObject()
    {
        return this.piece;
    }

    public void move_to_square(int new_square)
    {
        this.position_square = GameObject.Find("board " + new_square);
        this.square_number = new_square;
    }

    public void correct_movement_errors(int new_square)
    {
        Vector3 new_position = this.position_square.transform.position;

        //y position of the piece is higher than the position of the square (of 0.5).
        new_position = new_position + new Vector3(0, 0.5f, 0);
        this.piece.transform.position = new_position;
    }

    // move piece to a given point.
    public void move_to_point(Vector3 point)
    {
        this.piece.transform.position = point;
    }

    public void set_new_type(string name, GameObject appearence)
    {
        this.type = name;
        this.piece = appearence;
    }

    public bool get_alive_status()
    {
        return this.alive;
    }

    public void set_alive_status(bool status)
    {
        this.alive = status;
    }
}
